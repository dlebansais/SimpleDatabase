using Database.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Database.Internal
{
    internal interface IMySqlConnector : IDatabaseConnector
    {
    }

    internal class MySqlConnector : DatabaseConnector, IMySqlConnector
    {
        #region Constants
        private const int ER_DBACCESS_DENIED_ERROR = 1044;
        private const int ER_ACCESS_DENIED_ERROR = 1045;
        private const int ER_BAD_DB_ERROR = 1049;
        private const int ER_CANNOT_USER = 1396;
        #endregion

        #region Init
        public MySqlConnector()
        {
            ActiveOperationTable = new Dictionary<IMySqlActiveOperation, MySqlCommand>();
        }

        private MySqlConnection Connection;
        #endregion

        #region Credentials
        public override bool IsCredentialValid(ICredential credential, TimeSpan minDuration)
        {
            bool Success;
            StartWatch(out Stopwatch Watch);

            try
            {
                string ConnectionString = CredentialToConnectionString(credential, true);

                using (MySqlConnection VerificationConnection = new MySqlConnection(ConnectionString))
                {
                    VerificationConnection.Open();
                    VerificationConnection.Close();
                }

                Success = true;
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                bool IsValidityError = false;

                MySqlException InnerException = e;
                while (InnerException != null && !IsValidityError)
                    if (InnerException.Number == ER_DBACCESS_DENIED_ERROR || InnerException.Number == ER_ACCESS_DENIED_ERROR || InnerException.Number == ER_BAD_DB_ERROR)
                        IsValidityError = true;
                    else
                    {
                        _LastErrorCode = InnerException.Number;
                        InnerException = InnerException.InnerException as MySqlException;
                    }

                if (!IsValidityError)
                    TraceMySqlException(e);

                Success = false;
            }
            catch (Exception e)
            {
                TraceException(e);
                Success = false;
            }

            MinWait(Watch, minDuration);
            return Success;
        }

        public override bool CreateCredential(string rootId, string rootPassword, ICredential credential)
        {
            string ConnectionString;
            string CommandString;

            try
            {
                ICredential RootCredential = new Credential(credential.Server, rootId, rootPassword);
                ConnectionString = CredentialToConnectionString(RootCredential, false);
                bool Success = true;

                using (MySqlConnection RootConnection = new MySqlConnection(ConnectionString))
                {
                    RootConnection.Open();
                    ReadServerVersion(RootConnection);
                    //AnalyzeRootPrivileges(RootConnection, credential.Server, rootId);

                    if (ServerVersionMajor >= 8)
                    {
                        CommandString = $"CREATE USER IF NOT EXISTS '{credential.UserId}'@'{credential.Server}' IDENTIFIED BY '{credential.Password}';";
                        TraceCommand(CommandString);
                        Success &= ExecuteCommand(RootConnection, CommandString);
                    }
                    else
                    {
                        CommandString = $"SELECT 1 FROM mysql.user WHERE user = '{credential.UserId}';";
                        TraceCommand(CommandString);

                        if (!ExecuteQuery(RootConnection, CommandString, out int UserCount) || UserCount == 0)
                        {
                            CommandString = $"CREATE USER '{credential.UserId}'@'{credential.Server}' IDENTIFIED BY '{credential.Password}';";
                            TraceCommand(CommandString);
                            Success &= ExecuteCommand(RootConnection, CommandString);
                        }
                    }

                    CommandString = $"CREATE DATABASE IF NOT EXISTS {credential.Schema.Name} DEFAULT CHARACTER SET utf8;";
                    TraceCommand(CommandString);
                    Success &= ExecuteCommand(RootConnection, CommandString);

                    CommandString = $"GRANT ALL ON {credential.Schema.Name}.* TO '{credential.UserId}'@'{credential.Server}';";
                    TraceCommand(CommandString);
                    Success &= ExecuteCommand(RootConnection, CommandString);

                    RootConnection.Close();
                }

                return Success;
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                return false;
            }
            catch (Exception e)
            {
                TraceException(e);
                return false;
            }
        }

        public override void DeleteCredential(string rootId, string rootPassword, ICredential credential)
        {
            string ConnectionString;
            string CommandString;
            int ErrorCode;

            try
            {
                ICredential RootCredential = new Credential(credential.Server, rootId, rootPassword);
                ConnectionString = CredentialToConnectionString(RootCredential, false);

                using (MySqlConnection RootConnection = new MySqlConnection(ConnectionString))
                {
                    RootConnection.Open();
                    ReadServerVersion(RootConnection);

                    bool IsDatabaseDropped = false;

                    CommandString = $"SHOW TABLES IN {credential.Schema.Name};";
                    TraceCommand(CommandString);

                    if ((ExecuteQuery(RootConnection, CommandString, out int TableCount, out ErrorCode) && TableCount == 0) || ErrorCode == ER_BAD_DB_ERROR)
                    {
                        CommandString = $"DROP DATABASE IF EXISTS {credential.Schema.Name};";
                        TraceCommand(CommandString);

                        IsDatabaseDropped = ExecuteCommand(RootConnection, CommandString);
                    }

                    if (IsDatabaseDropped)
                    {
                        try
                        {
                            CommandString = $"REVOKE ALL ON {credential.Schema.Name}.* FROM '{credential.UserId}'@'{credential.Server}';";
                            TraceCommand(CommandString);
                            ExecuteCommand(RootConnection, CommandString, out ErrorCode);
                        }
                        catch
                        {
                        }

                        CommandString = $"DROP USER {credential.UserId}@{credential.Server};";
                        TraceCommand(CommandString);
                        bool Success = ExecuteCommand(RootConnection, CommandString, out ErrorCode) || ErrorCode == ER_CANNOT_USER;
                    }

                    RootConnection.Close();
                }
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
            }
            catch (Exception e)
            {
                TraceException(e);
            }
        }

        private static string CredentialToConnectionString(ICredential credential, bool withSchema)
        {
            string Result = "";

            Result += "Server=" + credential.Server;
            Result += ";UserId=" + credential.UserId;
            Result += ";Password=" + credential.Password;

            if (withSchema)
                Result += ";Database=" + credential.Schema.Name;

            return Result;
        }
        #endregion

        #region Schema
        public override bool CreateTables(ICredential credential)
        {
            string ConnectionString;
            string CommandString;

            try
            {
                ConnectionString = CredentialToConnectionString(credential, false);

                using (MySqlConnection RootConnection = new MySqlConnection(ConnectionString))
                {
                    RootConnection.Open();
                    ReadServerVersion(RootConnection);

                    CommandString = $"CREATE DATABASE IF NOT EXISTS {credential.Schema.Name} DEFAULT CHARACTER SET utf8;";
                    ExecuteCommand(RootConnection, CommandString);

                    CommandString = $"USE {credential.Schema.Name};";
                    ExecuteCommand(RootConnection, CommandString);

                    CreateSchemaTables(credential.Schema, RootConnection);

                    RootConnection.Close();
                }

                return true;
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                return false;
            }
            catch (Exception e)
            {
                TraceException(e);
                return false;
            }
        }

        public override void DeleteTables(ICredential credential)
        {
            string ConnectionString;
            string CommandString;
            int TableCount;
            int RowCount;
            int ErrorCode;

            try
            {
                ConnectionString = CredentialToConnectionString(credential, false);

                using (MySqlConnection RootConnection = new MySqlConnection(ConnectionString))
                {
                    RootConnection.Open();
                    ReadServerVersion(RootConnection);

                    CommandString = $"SHOW TABLES IN {credential.Schema.Name};";
                    TraceCommand(CommandString);

                    if (ExecuteQuery(RootConnection, CommandString, out TableCount, out ErrorCode) && TableCount > 0)
                    {
                        int EmptyTableCount = 0;
                        foreach (KeyValuePair<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Entry in credential.Schema.Tables)
                        {
                            ITableDescriptor Table = Entry.Key;
                            IColumnDescriptor PrimaryKey = Table.PrimaryKey;

                            CommandString = $"SELECT {PrimaryKey.Name} FROM {Table.Name};";
                            TraceCommand(CommandString);

                            if (ExecuteQuery(RootConnection, CommandString, out RowCount, out ErrorCode) && RowCount > 0)
                                break;

                            EmptyTableCount++;
                        }

                        if (EmptyTableCount == TableCount)
                        {
                            foreach (KeyValuePair<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Entry in credential.Schema.Tables)
                            {
                                ITableDescriptor Table = Entry.Key;

                                CommandString = $"DROP TABLE {Table.Name};";
                                TraceCommand(CommandString);

                                ExecuteCommand(RootConnection, CommandString, out ErrorCode);
                            }
                        }
                    }

                    RootConnection.Close();
                }
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
            }
            catch (Exception e)
            {
                TraceException(e);
            }
        }
        #endregion

        #region Open
        public override bool IsOpen { get { return Connection != null && Connection.State == ConnectionState.Open; } }

        public override bool Open(ICredential credential)
        {
            try
            {
                string ConnectionString = CredentialToConnectionString(credential, false);

                Connection = new MySqlConnection(ConnectionString);
                Connection.Open();

                if (IsOpen)
                {
                    ReadServerVersion(Connection);

                    string CreateCommandString = $"CREATE DATABASE IF NOT EXISTS {credential.Schema.Name} DEFAULT CHARACTER SET utf8;";
                    ExecuteCommand(Connection, CreateCommandString);

                    string UseCommandString = $"USE {credential.Schema.Name};";
                    ExecuteCommand(Connection, UseCommandString);

                    return true;
                }
                else
                    return false;
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                return false;
            }
            catch (Exception e)
            {
                TraceException(e);
                return false;
            }
        }
        #endregion

        #region Single Query
        public override IActiveOperation<ISingleQueryResultInternal> SingleQuery(ISingleQueryContext context)
        {
            IMySqlSingleQueryOperation Operation = new MySqlSingleQueryOperation(context);
            return PrepareReaderOperation<ISingleQueryContext, IMySqlSingleQueryOperation, ISingleQueryOperation, ISingleQueryResult, ISingleQueryResultInternal>(
                Operation, 
                (ISingleQueryOperation operation, IAsyncResult asyncResult) => new SingleQueryResult(operation, asyncResult),
                (MySqlCommand command, ISingleQueryResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Multi Query
        public override IActiveOperation<IJoinQueryResultInternal> JoinQuery(IJoinQueryContext context)
        {
            IMySqlJoinQueryOperation Operation = new MySqlJoinQueryOperation(context);
            return PrepareReaderOperation<IJoinQueryContext, IMySqlJoinQueryOperation, IJoinQueryOperation, IJoinQueryResult, IJoinQueryResultInternal>(
                Operation, 
                (IJoinQueryOperation operation, IAsyncResult asyncResult) => new JoinQueryResult(operation, asyncResult),
                (MySqlCommand command, IJoinQueryResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Update
        public override IActiveOperation<IUpdateResultInternal> Update(IUpdateContext context)
        {
            IMySqlUpdateOperation Operation = new MySqlUpdateOperation(context);
            IReadOnlyCollection<IColumnValuePair<byte[]>> DataEntryList = Operation.GetDataEntryList();
            return PrepareNonQueryOperationWithParameter<IUpdateContext, IMySqlUpdateOperation, IUpdateOperation, IUpdateResult, IUpdateResultInternal>(
                Operation, DataEntryList, 
                (IUpdateOperation operation, IAsyncResult asyncResult) => new UpdateResult(operation, asyncResult),
                (MySqlCommand command, IUpdateResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Insert
        public override IActiveOperation<IInsertResultInternal> Insert(IInsertContext context)
        {
            if (ServerVersionMajor < 8)
            {
                IColumnDescriptor PrimaryKey = context.Table.PrimaryKey;

                bool HasPrimary = false;
                foreach (IColumnValueCollectionPair Entry in context.EntryList)
                    if (Entry.Column == PrimaryKey)
                    {
                        HasPrimary = true;
                        break;
                    }

                if (!HasPrimary && (PrimaryKey is IColumnDescriptorGuid))
                    return new ActiveOperation<IInsertResultInternal>(new InsertResult(false));
            }

            IMySqlInsertOperation Operation = new MySqlInsertOperation(context);
            IReadOnlyCollection<IColumnValueCollectionPair<byte[]>> DataCollectionEntryList = Operation.GetDataEntryList();

            List<IColumnValuePair<byte[]>> DataEntryList = new List<IColumnValuePair<byte[]>>();
            foreach (IColumnValueCollectionPair<byte[]> Entry in DataCollectionEntryList)
                foreach (byte[] Value in Entry.ValueCollection)
                    DataEntryList.Add(new ColumnValuePair<byte[]>(Entry.Column, Value));

            return PrepareNonQueryOperationWithParameter<IInsertContext, IMySqlInsertOperation, IInsertOperation, IInsertResult, IInsertResultInternal>(
                Operation, DataEntryList,
                (IInsertOperation operation, IAsyncResult asyncResult) => new InsertResult(operation, asyncResult),
                (MySqlCommand command, IInsertResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Delete
        public override IActiveOperation<IDeleteResultInternal> Delete(IDeleteContext context)
        {
            IMySqlDeleteOperation Operation = new MySqlDeleteOperation(context);
            return PrepareNonQueryOperation<IDeleteContext, IMySqlDeleteOperation, IDeleteOperation, IDeleteResult, IDeleteResultInternal>(
                Operation, (IDeleteOperation operation, IAsyncResult asyncResult) => new DeleteResult(operation, asyncResult),
                (MySqlCommand command, IDeleteResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Operations
        protected virtual IActiveOperation<TInternal> PrepareReaderOperation<TContext, TMySqlOperation, TOperation, TResult, TInternal>(TMySqlOperation operation, Func<TOperation, IAsyncResult, TInternal> createResultHandler, Func<MySqlCommand, TInternal, string> finalizer)
            where TContext : IQueryContext
            where TMySqlOperation : IMySqlQueryOperation<TContext, TInternal>, TOperation
            where TOperation : IQueryOperation, IOperation<TContext, TResult>
            where TResult : IQueryResult
            where TInternal : IQueryResultInternal, TResult
        {
            TInternal OperationResult;
            MySqlActiveOperation<TInternal> ActiveOperation;

            try
            {
                string QueryString = operation.GetCommandText();

                MySqlCommand Command = new MySqlCommand(QueryString, Connection);

                TraceCommand(Command);
                IAsyncResult Result = Command.BeginExecuteReader();

                OperationResult = createResultHandler(operation, Result);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult, finalizer);
                ActiveOperationTable.Add(ActiveOperation, Command);
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                OperationResult = createResultHandler(operation, null);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult);
            }
            catch (Exception e)
            {
                TraceException(e);
                OperationResult = createResultHandler(operation, null);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult);
            }

            return ActiveOperation;
        }

        protected virtual IActiveOperation<TInternal> PrepareNonQueryOperation<TContext, TMySqlOperation, TOperation, TResult, TInternal>(TMySqlOperation operation, Func<TOperation, IAsyncResult, TInternal> createResultHandler, Func<MySqlCommand, TInternal, string> finalizer)
            where TContext : IModifyContext
            where TMySqlOperation : IMySqlModifyOperation<TContext, TInternal>, TOperation
            where TOperation : IModifyOperation, IOperation<TContext, TResult>
            where TResult : IModifyResult
            where TInternal : IModifyResultInternal, TResult
        {
            TInternal OperationResult;
            MySqlActiveOperation<TInternal> ActiveOperation;

            try
            {
                string CommandText = operation.GetCommandText();

                MySqlCommand Command = new MySqlCommand(CommandText, Connection);

                TraceCommand(Command);
                IAsyncResult Result = Command.BeginExecuteNonQuery();

                OperationResult = createResultHandler(operation, Result);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult, finalizer);
                ActiveOperationTable.Add(ActiveOperation, Command);
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                OperationResult = createResultHandler(operation, null);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult);
            }
            catch (Exception e)
            {
                TraceException(e);
                OperationResult = createResultHandler(operation, null);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult);
            }

            return ActiveOperation;
        }

        protected virtual IActiveOperation<TInternal> PrepareNonQueryOperationWithParameter<TContext, TMySqlOperation, TOperation, TResult, TInternal>(TMySqlOperation operation, IReadOnlyCollection<IColumnValuePair<byte[]>> dataEntryList, Func<TOperation, IAsyncResult, TInternal> createResultHandler, Func<MySqlCommand, TInternal, string> finalizer)
            where TContext : IModifyContext, IDataContext
            where TMySqlOperation : IMySqlModifyOperation<TContext, TInternal>, TOperation
            where TOperation : IModifyOperation, IOperation<TContext, TResult>
            where TResult : IModifyResult, IDataResult
            where TInternal : IModifyResultInternal, IDataResultInternal, TResult
        {
            TInternal OperationResult;
            MySqlActiveOperation<TInternal> ActiveOperation;

            try
            {
                string CommandText = operation.GetCommandText();

                MySqlCommand Command = new MySqlCommand(CommandText, Connection);

                int i = 0;
                foreach (IColumnValuePair<byte[]> DataEntry in dataEntryList)
                {
                    MySqlParameter DataParameter = Command.CreateParameter();
                    DataParameter.ParameterName = $"data{i}";
                    DataParameter.Value = DataEntry.Value;
                    Command.Parameters.Add(DataParameter);

                    i++;
                }

                TraceCommand(Command);
                IAsyncResult Result = Command.BeginExecuteNonQuery();

                OperationResult = createResultHandler(operation, Result);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult, finalizer);
                ActiveOperationTable.Add(ActiveOperation, Command);
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                OperationResult = createResultHandler(operation, null);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult);
            }
            catch (Exception e)
            {
                TraceException(e);
                OperationResult = createResultHandler(operation, null);
                ActiveOperation = new MySqlActiveOperation<TInternal>(OperationResult);
            }

            return ActiveOperation;
        }

        public override void NotifyOperationCompleted(IActiveOperation activeOperation)
        {
            if (activeOperation is IMySqlActiveOperation AsMySqlActiveOperation)
            {
                try
                {
                    using (MySqlCommand Command = ActiveOperationTable[AsMySqlActiveOperation])
                    {
                        ActiveOperationTable.Remove(AsMySqlActiveOperation);
                        string Diagnostic = AsMySqlActiveOperation.MySqlFinalizerBase(Command, activeOperation.ResultBase);

                        TraceCommandEnd(Command, Diagnostic);
                    }
                }
#if TRACE
                catch (ApplicationException)
                {
                    throw;
                }
#endif
                catch (MySqlException e)
                {
                    TraceMySqlException(e);
                }
                catch (Exception e)
                {
                    TraceException(e);
                }
            }
            else
                throw new InvalidCastException("Invalid operation");
        }

        private static void TraceCommand(MySqlCommand command)
        {
            Debugging.Print($"MySql command ({command.GetHashCode()}): {command.CommandText}");
        }

        private static void TraceCommand(string commandText)
        {
            Debugging.Print($"MySql command: {commandText}");
        }

        private static void TraceCommandEnd(MySqlCommand command, string diagnostic)
        {
            Debugging.Print($"MySql command ({command.GetHashCode()}) {diagnostic}");
        }

        private static void TraceMySqlException(MySqlException e, [CallerMemberName] string CallerName = "")
        {
            _LastErrorCode = e.Number;

            string CallerNameInfo = (CallerName.Length > 0 ? $" (from {CallerName})" : "");
            string ServerVersionInfo = (ServerVersion != null ? $", Server Version: {ServerVersion}" : "");
            string Message = $"MySql exception{CallerNameInfo}{ServerVersionInfo}: {e.Message}, number={e.Number}";

            if (e.Number == _IgnoreErrorCode)
            {
                _IgnoreErrorCode = 0;
                Debugging.Print(Message);
            }
            else
                Debugging.PrintExceptionMessage(Message);
        }

        private static void TraceException(Exception e, [CallerMemberName] string CallerName = "")
        {
            string CallerNameInfo = (CallerName.Length > 0 ? $" (from {CallerName})" : "");
            string ServerVersionInfo = (ServerVersion != null ? $", Server Version: {ServerVersion}" : "");
            string Message = $"Exception{CallerNameInfo}{ServerVersionInfo}: {e.Message}";

            Debugging.PrintExceptionMessage(Message);
        }

        private void StartWatch(out Stopwatch watch)
        {
            watch = new Stopwatch();
            watch.Start();
        }

        private void MinWait(Stopwatch watch, TimeSpan duration)
        {
            TimeSpan Remaining = duration - watch.Elapsed;
            if (Remaining > TimeSpan.Zero)
                Thread.Sleep(Remaining);
        }

        public override int IgnoreErrorCode { get { return _IgnoreErrorCode; } set { _IgnoreErrorCode = value; } }
        private static int _IgnoreErrorCode { get; set; }
        public override int LastErrorCode { get { return _LastErrorCode; } }
        private static int _LastErrorCode;
        public override bool CanIntBeNULL { get { return ServerVersionMajor >= 8; } }

        private Dictionary<IMySqlActiveOperation, MySqlCommand> ActiveOperationTable;
        private static string ServerVersion;
        private static int ServerVersionMajor;
        private static int ServerVersionMinor;
        #endregion

        #region Close
        public override void Close()
        {
            try
            {
                Connection.Close();
                Connection = null;
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
            }
            catch (Exception e)
            {
                TraceException(e);
            }
        }
        #endregion

        #region Initialization
        public static void ReadServerVersion(MySqlConnection rootConnection)
        {
            if (ServerVersion != null)
                return;

            ServerVersion = "Unknown";

            try
            {
                using (MySqlCommand Command = new MySqlCommand("SELECT VERSION()", rootConnection))
                {
                    using (MySqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.Read())
                        {
                            string s = Reader["VERSION()"] as string;
                            if (!string.IsNullOrEmpty(s))
                            {
                                ServerVersion = s;

                                string[] Splitted = s.Split('.');
                                if (Splitted.Length >= 2 && int.TryParse(Splitted[0], out int Major) && int.TryParse(Splitted[1], out int Minor))
                                {
                                    ServerVersionMajor = Major;
                                    ServerVersionMinor= Minor;
                                    ServerVersion = $"[{Major}.{Minor}] {ServerVersion}";
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void AnalyzeRootPrivileges(MySqlConnection rootConnection, string server, string rootId)
        {
            try
            {
                string Result = "RootPrivileges:\n";

                using (MySqlCommand Command = new MySqlCommand($"SELECT * FROM mysql.user WHERE User='{rootId}' and Host='{server}'", rootConnection))
                {
                    using (MySqlDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            for (int i = 0; i < Reader.FieldCount; i++)
                                Result += $"{Reader.GetName(i)}: {Reader[i]}\n";

                            Result += $"****\n";
                        }
                    }
                }

                Debugging.Print(Result);
            }
            catch
            {
            }
        }

        private static bool ExecuteCommand(MySqlConnection rootConnection, string commandString)
        {
            return ExecuteCommand(rootConnection, commandString, out int ErrorCode);
        }

        private static bool ExecuteCommand(MySqlConnection rootConnection, string commandString, out int errorCode)
        {
            try
            {
                using (MySqlCommand Command = new MySqlCommand(commandString, rootConnection))
                {
                    int Result = Command.ExecuteNonQuery();
                    errorCode = 0;
                    return true;
                }
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                errorCode = e.Number;
                return false;
            }
            catch (Exception e)
            {
                TraceException(e);
                errorCode = -1;
                return false;
            }
        }

        public static bool ExecuteQuery(MySqlConnection rootConnection, string commandString, out int rowCount)
        {
            return ExecuteQuery(rootConnection, commandString, out rowCount, out int errorCode);
        }

        public static bool ExecuteQuery(MySqlConnection rootConnection, string commandString, out int rowCount, out int errorCode)
        {
            try
            {
                using (MySqlCommand Command = new MySqlCommand(commandString, rootConnection))
                {
                    using (MySqlDataReader Reader = Command.ExecuteReader())
                    {
                        rowCount = 0;
                        while (Reader.Read())
                            rowCount++;

                        errorCode = 0;
                        return true;
                    }
                }
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (MySqlException e)
            {
                TraceMySqlException(e);
                errorCode = e.Number;
                rowCount = -1;
                return false;
            }
            catch (Exception e)
            {
                TraceException(e);
                errorCode = -1;
                rowCount = -1;
                return false;
            }
        }

        private static void CreateSchemaTables(ISchemaDescriptor schema, MySqlConnection rootConnection)
        {
            foreach (KeyValuePair<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> TableEntry in schema.Tables)
            {
                ITableDescriptor Table = TableEntry.Key;
                string TableName = Table.Name;
                Debug.Assert(TableName.Length > 0);

                IReadOnlyCollection<IColumnDescriptor> TableStructure = TableEntry.Value;
                Debug.Assert(TableStructure.Count > 0);

                string KeyColumn = null;
                bool IsAutoIncrement = false;
                Dictionary<string, string> ColumnTable = new Dictionary<string, string>();
                foreach (IColumnDescriptor ColumnEntry in TableStructure)
                {
                    string ColumnName = ColumnEntry.Name;
                    string TypeName = ColumnEntry.Type.SqlType;
                    ColumnTable.Add(ColumnName, TypeName);

                    if (KeyColumn == null)
                    {
                        KeyColumn = ColumnName;
                        IsAutoIncrement = (ColumnEntry.Type is ColumnTypeInt);
                    }
                }

                Debug.Assert(KeyColumn != null);

                string ColumnStringList = "";
                foreach (KeyValuePair<string, string> ColumnEntry in ColumnTable)
                {
                    string ColumnName = ColumnEntry.Key;
                    string TypeName = ColumnEntry.Value;

                    if (ColumnStringList.Length > 0)
                        ColumnStringList += ", ";

                    ColumnStringList += ColumnName + " " + TypeName;

                    if (ColumnName == KeyColumn)
                    {
                        ColumnStringList += " NOT NULL";

                        if (IsAutoIncrement)
                            ColumnStringList += " AUTO_INCREMENT";

                        ColumnStringList += " KEY";
                    }
                }

                string CommandString = $"CREATE TABLE IF NOT EXISTS {TableName} ( { ColumnStringList} );";
                TraceCommand(CommandString);
                ExecuteCommand(rootConnection, CommandString);
            }
        }
        #endregion
    }
}
