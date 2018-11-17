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
        #region Init
        public MySqlConnector()
        {
            ActiveOperationTable = new Dictionary<IMySqlActiveOperation, MySqlCommand>();
        }

        private MySqlConnection Connection;
        #endregion

        #region Schema
        public override bool CreateTables(ICredential credential, ISchemaDescriptor schema)
        {
            string ConnectionString;
            string CommandString;

            try
            {
                ConnectionString = CredentialToConnectionString(credential, false);

                using (MySqlConnection RootConnection = new MySqlConnection(ConnectionString))
                {
                    RootConnection.Open();

                    CommandString = "CREATE DATABASE IF NOT EXISTS " + credential.Schema.Name + " DEFAULT CHARACTER SET utf8" + ";";
                    ExecuteCommand(RootConnection, CommandString);

                    CommandString = "USE " + credential.Schema.Name + ";";
                    ExecuteCommand(RootConnection, CommandString);

                    CreateSchemaTables(schema, RootConnection);

                    RootConnection.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                TraceMySqlException(e);
                return false;
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
                    string CreateCommandString = "CREATE DATABASE IF NOT EXISTS " + credential.Schema.Name + " DEFAULT CHARACTER SET utf8" + ";";
                    ExecuteCommand(Connection, CreateCommandString);

                    string UseCommandString = "USE " + credential.Schema.Name + ";";
                    ExecuteCommand(Connection, UseCommandString);

                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                TraceMySqlException(e);
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
        public override IActiveOperation<IMultiQueryResultInternal> MultiQuery(IMultiQueryContext context)
        {
            IMySqlMultiQueryOperation Operation = new MySqlMultiQueryOperation(context);
            return PrepareReaderOperation<IMultiQueryContext, IMySqlMultiQueryOperation, IMultiQueryOperation, IMultiQueryResult, IMultiQueryResultInternal>(
                Operation, 
                (IMultiQueryOperation operation, IAsyncResult asyncResult) => new MultiQueryResult(operation, asyncResult),
                (MySqlCommand command, IMultiQueryResultInternal result) => Operation.FinalizeOperation(command, result));
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

        #region Single Insert
        public override IActiveOperation<ISingleInsertResultInternal> SingleInsert(ISingleInsertContext context)
        {
            IMySqlSingleInsertOperation Operation = new MySqlSingleInsertOperation(context);
            IReadOnlyCollection<IColumnValuePair<byte[]>> DataEntryList = Operation.GetDataEntryList();
            return PrepareNonQueryOperationWithParameter<ISingleInsertContext, IMySqlSingleInsertOperation, ISingleInsertOperation, ISingleInsertResult, ISingleInsertResultInternal>(
                Operation, DataEntryList, 
                (ISingleInsertOperation operation, IAsyncResult asyncResult) => new SingleInsertResult(operation, asyncResult),
                (MySqlCommand command, ISingleInsertResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Multi Insert
        public override IActiveOperation<IMultiInsertResultInternal> MultiInsert(IMultiInsertContext context)
        {
            IMySqlMultiInsertOperation Operation = new MySqlMultiInsertOperation(context);
            return PrepareNonQueryOperation<IMultiInsertContext, IMySqlMultiInsertOperation, IMultiInsertOperation, IMultiInsertResult, IMultiInsertResultInternal>(
                Operation, 
                (IMultiInsertOperation operation, IAsyncResult asyncResult) => new MultiInsertResult(operation, asyncResult),
                (MySqlCommand command, IMultiInsertResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Single Row Delete
        public override IActiveOperation<ISingleRowDeleteResultInternal> SingleRowDelete(ISingleRowDeleteContext context)
        {
            IMySqlSingleRowDeleteOperation Operation = new MySqlSingleRowDeleteOperation(context);
            return PrepareNonQueryOperation<ISingleRowDeleteContext, IMySqlSingleRowDeleteOperation, ISingleRowDeleteOperation, ISingleRowDeleteResult, ISingleRowDeleteResultInternal>(
                Operation, (ISingleRowDeleteOperation operation, IAsyncResult asyncResult) => new SingleRowDeleteResult(operation, asyncResult),
                (MySqlCommand command, ISingleRowDeleteResultInternal result) => Operation.FinalizeOperation(command, result));
        }
        #endregion

        #region Multi Row Delete
        public override IActiveOperation<IMultiRowDeleteResultInternal> MultiRowDelete(IMultiRowDeleteContext context)
        {
            IMySqlMultiRowDeleteOperation Operation = new MySqlMultiRowDeleteOperation(context);
            return PrepareNonQueryOperation<IMultiRowDeleteContext, IMySqlMultiRowDeleteOperation, IMultiRowDeleteOperation, IMultiRowDeleteResult, IMultiRowDeleteResultInternal>(
                Operation, (IMultiRowDeleteOperation operation, IAsyncResult asyncResult) => new MultiRowDeleteResult(operation, asyncResult),
                (MySqlCommand command, IMultiRowDeleteResultInternal result) => Operation.FinalizeOperation(command, result));
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
            catch (Exception e)
            {
                TraceMySqlException(e);
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
            catch (Exception e)
            {
                TraceMySqlException(e);
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
            catch (Exception e)
            {
                TraceMySqlException(e);
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
                catch (Exception e)
                {
                    TraceMySqlException(e);
                }
            }
            else
                throw new InvalidCastException("Invalid operation");
        }

        private void TraceCommand(MySqlCommand command)
        {
            Debugging.Print($"MySql command ({command.GetHashCode()}): {command.CommandText}");
        }

        private void TraceCommandEnd(MySqlCommand command, string diagnostic)
        {
            Debugging.Print($"MySql command ({command.GetHashCode()}) {diagnostic}");
        }

        private void TraceMySqlException(Exception e, [CallerMemberName] string CallerName = "")
        {
            Debugging.Print("MySql exception" + (CallerName.Length > 0 ? " (from " + CallerName + ")" : "") + ": " + e.Message);
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

        private Dictionary<IMySqlActiveOperation, MySqlCommand> ActiveOperationTable;
        #endregion

        #region Close
        public override void Close()
        {
            try
            {
                Connection.Close();
                Connection = null;
            }
            catch (Exception e)
            {
                TraceMySqlException(e);
            }
        }
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
            catch (Exception e)
            {
                TraceMySqlException(e);
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

                using (MySqlConnection RootConnection = new MySqlConnection(ConnectionString))
                {
                    RootConnection.Open();

                    CommandString = "CREATE USER " + "'" + credential.UserId + "'" + "@" + "'" + "localhost" + "'" + " IDENTIFIED BY " + "'" + credential.Password + "'" + ";";
                    ExecuteCommand(RootConnection, CommandString);

                    CommandString = "CREATE DATABASE IF NOT EXISTS " + credential.Schema.Name + " DEFAULT CHARACTER SET utf8" + ";";
                    ExecuteCommand(RootConnection, CommandString);

                    CommandString = "GRANT ALL ON " + credential.Schema.Name + ".* TO " + "'" + credential.UserId + "'" + "@" + "'" + "localhost" + "'" + ";";
                    ExecuteCommand(RootConnection, CommandString);

                    RootConnection.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                TraceMySqlException(e);
                return false;
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

        #region Initialization
        public static void ExecuteCommand(MySqlConnection RootConnection, string CommandString)
        {
            try
            {
                using (MySqlCommand Command = new MySqlCommand(CommandString, RootConnection))
                {
                    int Result = Command.ExecuteNonQuery();
                }
            }
            catch
            {
            }
        }

        private static void CreateSchemaTables(ISchemaDescriptor schema, MySqlConnection RootConnection)
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

                string QueryString = "CREATE TABLE IF NOT EXISTS " + TableName + " ( " + ColumnStringList + " );";
                ExecuteCommand(RootConnection, QueryString);
            }
        }
        #endregion
    }
}
