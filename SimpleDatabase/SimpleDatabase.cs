using Database.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Database
{
    #region Interface
    /// <summary>
    ///     An interface to connect to a database and perform queries on it.
    /// </summary>
    public interface ISimpleDatabase : IDisposable
    {
        /// <summary>
        ///     Initializes the database, loading the engine associated to <paramref name="connectorType"/>.
        /// </summary>
        /// <parameters>
        /// <param name="connectorType">The engine backing the database.</param>
        /// <param name="connectionOption">Options related to connection to a server.</param>
        /// </parameters>
        void Initialize(ConnectorType connectorType, ConnectionOption connectionOption);

        /// <summary>
        ///     Gets the engine backing the database.
        /// </summary>
        /// <returns>
        ///     The engine backing the database.
        /// </returns>
        ConnectorType ConnectorType { get; }

        /// <summary>
        ///     Gets the options related to connection to a server
        /// </summary>
        /// <returns>
        ///     The options related to connection to a server
        /// </returns>
        ConnectionOption ConnectionOption { get; }

        /// <summary>
        ///     Performs checks on a <see cref="ICredential"/> object to verify if it can be used with the <see cref="Open"/> method successfully.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential to verify.</param>
        /// </parameters>
        /// <returns>
        ///     True if <paramref name="credential"/> can be used with the <see cref="Open"/> method successfully. False if it would fail.
        /// </returns>
        bool IsCredentialValid(ICredential credential);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<bool> IsCredentialValidAsync(ICredential credential);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Performs checks on a <see cref="ICredential"/> object to verify if it can be used with the <see cref="Open"/> method successfully.
        ///     Since this operation can take some time or be fast, this method has a <paramref name="minDuration"/> parameter to improve the user experience.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential to verify.</param>
        /// <param name="minDuration">Minimum duration this operation should take.</param>
        /// </parameters>
        /// <returns>
        ///     True if <paramref name="credential"/> can be used with the <see cref="Open"/> method successfully. False if it would fail.
        /// </returns>
        Task<bool> IsCredentialValidAsync(ICredential credential, TimeSpan minDuration);

        /// <summary>
        ///     Creates a new user in the database, using information from <paramref name="credential"/>.
        ///     The default schema, if any, is also created with no tables.
        /// </summary>
        /// <parameters>
        /// <param name="rootId">The root identifier.</param>
        /// <param name="rootPassword">The root password.</param>
        /// <param name="credential">Information about the user to create.</param>
        /// </parameters>
        /// <returns>
        ///     True if the user could be created. False if the operation failed.
        /// </returns>
        bool CreateCredential(string rootId, string rootPassword, ICredential credential);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<bool> CreateCredentialAsync(string rootId, string rootPassword, ICredential credential);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Creates or updates a schema.
        ///     The schema name used is from <paramref name="credential"/> default schema.
        ///     If it already exists, only new tables are created, and existing data is preserved.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential used to perform the operation.</param>
        /// </parameters>
        /// <returns>
        ///     True if the schema could be created or updated. False if the operation failed.
        /// </returns>
        bool CreateTables(ICredential credential);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<bool> CreateTablesAsync(ICredential credential);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Deletes a schema. All tables must be empty.
        ///     The schema name used is from <paramref name="credential"/> default schema.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential used to perform the operation.</param>
        /// </parameters>
        void DeleteTables(ICredential credential);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task DeleteTablesAsync(ICredential credential);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Delete an existing user and schema in the database, using information from <paramref name="credential"/>, if the schema contains no tables.
        /// </summary>
        /// <parameters>
        /// <param name="rootId">The root identifier.</param>
        /// <param name="rootPassword">The root password.</param>
        /// <param name="credential">Information about the user to delete.</param>
        /// </parameters>
        void DeleteCredential(string rootId, string rootPassword, ICredential credential);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task DeleteCredentialAsync(string rootId, string rootPassword, ICredential credential);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Opens a database to perform operations on.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential used to open the database.</param>
        /// </parameters>
        /// <returns>
        ///     True if open succeeded. False if the operation failed.
        /// </returns>
        bool Open(ICredential credential);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<bool> OpenAsync(ICredential credential);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Indicates if the database is opened.
        /// </summary>
        /// <returns>
        ///     True if database is opened. False otherwise.
        /// </returns>
        bool IsOpen { get; }

        /// <summary>
        ///     Closes a database opened with the <see cref="Open"/> method.
        /// </summary>
        void Close();
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task CloseAsync();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Executes a request that queries values on a single table, with constraints.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        ISingleQueryResult Run(ISingleQueryContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<ISingleQueryResult> RunAsync(ISingleQueryContext context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Executes a request request to query values from several tables.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        IMultiQueryResult Run(IMultiQueryContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<IMultiQueryResult> RunAsync(IMultiQueryContext context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Executes a request to update values in a row of a table, with constraints on the previous values.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        IUpdateResult Run(IUpdateContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<IUpdateResult> RunAsync(IUpdateContext context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Executes a request to insert a single row of values in a table.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        ISingleInsertResult Run(ISingleInsertContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<ISingleInsertResult> RunAsync(ISingleInsertContext context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Executes a request to insert several values in a table.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        IMultiInsertResult Run(IMultiInsertContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<IMultiInsertResult> RunAsync(IMultiInsertContext context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Executes a request to delete rows in a table, with constraints on some values.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        ISingleRowDeleteResult Run(ISingleRowDeleteContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<ISingleRowDeleteResult> RunAsync(ISingleRowDeleteContext context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        ///     Executes a request to delete rows in a table, with constraints.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        IMultiRowDeleteResult Run(IMultiRowDeleteContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<IMultiRowDeleteResult> RunAsync(IMultiRowDeleteContext context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
    #endregion

    /// <summary>
    ///     A class to connect to a database and perform queries on it.
    /// </summary>
    public class SimpleDatabase : ISimpleDatabase, IDisposable
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleDatabase"/> class.
        /// </summary>
        public SimpleDatabase()
        {
            Connector = null;
            ConnectionMutex = new Mutex();
        }

        /// <summary>
        ///     Initializes the database, loading the engine associated to <paramref name="connectorType"/>.
        /// </summary>
        /// <parameters>
        /// <param name="connectorType">The engine backing the database.</param>
        /// <param name="connectionOption">Options related to connection to a server.</param>
        /// </parameters>
        public virtual void Initialize(ConnectorType connectorType, ConnectionOption connectionOption)
        {
            Debug.Assert(Connector == null);
            if (Connector != null)
                return;

            ConnectorType = connectorType;
            ConnectionOption = connectionOption;

            try
            {
                switch (ConnectorType)
                {
                    case ConnectorType.MySql:
                        Connector = new MySqlConnector();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Invalid ConnectorType");
                }

                Debugging.Print("Connector created: " + Connector.ToString());
            }
            catch (ApplicationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Debugging.PrintExceptionMessage(e.Message);
            }
        }

        private IDatabaseConnector Connector;
        private Mutex ConnectionMutex;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the engine backing the database.
        /// </summary>
        /// <returns>
        ///     The engine backing the database.
        /// </returns>
        public virtual ConnectorType ConnectorType { get; private set; }

        /// <summary>
        ///     Gets the options related to connection to a server
        /// </summary>
        /// <returns>
        ///     The options related to connection to a server
        /// </returns>
        public virtual ConnectionOption ConnectionOption { get; private set; }
        #endregion

        #region Credentials
        /// <summary>
        ///     Performs checks on a <see cref="ICredential"/> object to verify if it can be used with the <see cref="Open"/> method successfully.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential to verify.</param>
        /// </parameters>
        /// <returns>
        ///     True if <paramref name="credential"/> can be used with the <see cref="Open"/> method successfully. False if it would fail.
        /// </returns>
        public virtual bool IsCredentialValid(ICredential credential)
        {
            return Connector.IsCredentialValid(credential, TimeSpan.Zero);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual Task<bool> IsCredentialValidAsync(ICredential credential)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return IsCredentialValidAsync(credential, TimeSpan.Zero);
        }

        /// <summary>
        ///     Performs checks on a <see cref="ICredential"/> object to verify if it can be used with the <see cref="Open"/> method successfully.
        ///     Since this operation can take some time or be fast, this method has a <paramref name="minDuration"/> parameter to improve the user experience.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential to verify.</param>
        /// <param name="minDuration">Minimum duration this operation should take.</param>
        /// </parameters>
        /// <returns>
        ///     True if <paramref name="credential"/> can be used with the <see cref="Open"/> method successfully. False if it would fail.
        /// </returns>
        public virtual Task<bool> IsCredentialValidAsync(ICredential credential, TimeSpan minDuration)
        {
            return Task.Run(() => Connector.IsCredentialValid(credential, minDuration));
        }

        /// <summary>
        ///     Creates a new user in the database, using information from <paramref name="credential"/>.
        ///     The default schema, if any, is also created with no tables.
        /// </summary>
        /// <parameters>
        /// <param name="rootId">The root identifier.</param>
        /// <param name="rootPassword">The root password.</param>
        /// <param name="credential">Information about the user to create.</param>
        /// </parameters>
        /// <returns>
        ///     True if the user could be created. False if the operation failed.
        /// </returns>
        public virtual bool CreateCredential(string rootId, string rootPassword, ICredential credential)
        {
            return Connector.CreateCredential(rootId, rootPassword, credential);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual Task<bool> CreateCredentialAsync(string rootId, string rootPassword, ICredential credential)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return Task.Run(() => Connector.CreateCredential(rootId, rootPassword, credential));
        }

        /// <summary>
        ///     Delete an existing user and schema in the database, using information from <paramref name="credential"/>, if the schema contains no tables.
        /// </summary>
        /// <parameters>
        /// <param name="rootId">The root identifier.</param>
        /// <param name="rootPassword">The root password.</param>
        /// <param name="credential">Information about the user to delete.</param>
        /// </parameters>
        public void DeleteCredential(string rootId, string rootPassword, ICredential credential)
        {
            Connector.DeleteCredential(rootId, rootPassword, credential);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task DeleteCredentialAsync(string rootId, string rootPassword, ICredential credential)
        {
            return Task.Run(() => Connector.DeleteCredential(rootId, rootPassword, credential));
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        #region Schema
        /// <summary>
        ///     Creates or updates a schema.
        ///     The schema name used is from <paramref name="credential"/> default schema.
        ///     If it already exists, only new tables are created, and existing data is preserved.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential used to perform the operation.</param>
        /// </parameters>
        /// <returns>
        ///     True if the schema could be created or updated. False if the operation failed.
        /// </returns>
        public virtual bool CreateTables(ICredential credential)
        {
            return Connector.CreateTables(credential);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual Task<bool> CreateTablesAsync(ICredential credential)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return Task.Run(() => Connector.CreateTables(credential));
        }

        /// <summary>
        ///     Deletes a schema. All tables must be empty.
        ///     The schema name used is from <paramref name="credential"/> default schema.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential used to perform the operation.</param>
        /// </parameters>
        public virtual void DeleteTables(ICredential credential)
        {
            Connector.DeleteTables(credential);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual Task DeleteTablesAsync(ICredential credential)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return Task.Run(() => Connector.DeleteTables(credential));
        }
        #endregion

        #region Open
        /// <summary>
        ///     Opens a database to perform operations on.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential used to open the database.</param>
        /// </parameters>
        /// <returns>
        ///     True if open succeeded. False if the operation failed.
        /// </returns>
        public virtual bool Open(ICredential credential)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(!Connector.IsOpen);

            return ExecuteOpen(credential);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual Task<bool> OpenAsync(ICredential credential)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(!Connector.IsOpen);

            return Task.Run(() => ExecuteOpen(credential));
        }

        /// <summary>
        ///     Opens a database to perform operations on.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="credential">The credential used to open the database.</param>
        /// </parameters>
        /// <returns>
        ///     True if open succeeded. False if the operation failed.
        /// </returns>
        protected virtual bool ExecuteOpen(ICredential credential)
        {
            ConnectionMutex.WaitOne(Timeout.InfiniteTimeSpan);

            bool Success;
            if (Connector.Open(credential))
            {
                Debug.Assert(Connector.IsOpen);

                StartOperationThread();
                Success = true;
            }
            else
            {
                ConnectionMutex.ReleaseMutex();
                Success = false;
            }

            return Success;
        }

        /// <summary>
        ///     Indicates if the database is opened.
        /// </summary>
        /// <returns>
        ///     True if database is opened. False otherwise.
        /// </returns>
        public virtual bool IsOpen
        {
            get { return Connector != null && Connector.IsOpen; } 
        }
        #endregion

        #region Single Query
        /// <summary>
        ///     Executes a request that queries values on a single table, with constraints.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public ISingleQueryResult Run(ISingleQueryContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<ISingleQueryResult> RunAsync(ISingleQueryContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => Execute(context));
        }

        /// <summary>
        ///     Executes a request that queries values on a single table, with constraints.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        protected virtual ISingleQueryResult Execute(ISingleQueryContext context)
        {
            if (!IsOperationThreadStarted)
                return new SingleQueryResult(false);

            IActiveOperation<ISingleQueryResultInternal> ActiveOperation = Connector.SingleQuery(context);
            ISingleQueryResultInternal Result = ActiveOperation.Result;

            if (Result.IsStarted)
            {
                ActiveOperationTable.Add(ActiveOperation);
                NewOperationEvent.Set();
                Result.WaitCompleted();
            }

            return ActiveOperation.Result;
        }
        #endregion

        #region Multi Query
        /// <summary>
        ///     Executes a request request to query values from several tables.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public IMultiQueryResult Run(IMultiQueryContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<IMultiQueryResult> RunAsync(IMultiQueryContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => Execute(context));
        }

        /// <summary>
        ///     Executes a request request to query values from several tables.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        protected virtual IMultiQueryResult Execute(IMultiQueryContext context)
        {
            if (!IsOperationThreadStarted)
                return new MultiQueryResult(false);

            IActiveOperation<IMultiQueryResultInternal> ActiveOperation = Connector.MultiQuery(context);
            IMultiQueryResultInternal Result = ActiveOperation.Result;

            if (Result.IsStarted)
            {
                ActiveOperationTable.Add(ActiveOperation);
                NewOperationEvent.Set();
                Result.WaitCompleted();
            }

            return Result;
        }
        #endregion

        #region Update
        /// <summary>
        ///     Executes a request to update values in a row of a table, with constraints on the previous values.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public IUpdateResult Run(IUpdateContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<IUpdateResult> RunAsync(IUpdateContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => Execute(context));
        }

        /// <summary>
        ///     Executes a request to update values in a row of a table, with constraints on the previous values.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        protected virtual IUpdateResult Execute(IUpdateContext context)
        {
            if (!IsOperationThreadStarted)
                return new UpdateResult(false);

            IActiveOperation<IUpdateResultInternal> ActiveOperation = Connector.Update(context);
            IUpdateResultInternal Result = ActiveOperation.Result;

            if (Result.IsStarted)
            {
                ActiveOperationTable.Add(ActiveOperation);
                NewOperationEvent.Set();
                Result.WaitCompleted();
            }

            return Result;
        }
        #endregion

        #region Single Insert
        /// <summary>
        ///     Executes a request to insert a single row of values in a table.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public ISingleInsertResult Run(ISingleInsertContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<ISingleInsertResult> RunAsync(ISingleInsertContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => Execute(context));
        }

        /// <summary>
        ///     Executes a request to insert a single row of values in a table.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        protected virtual ISingleInsertResult Execute(ISingleInsertContext context)
        {
            if (!IsOperationThreadStarted)
                return new SingleInsertResult(false);

            if (context.EntryList.Count == 0)
                return new SingleInsertResult(true);

            IActiveOperation<ISingleInsertResultInternal> ActiveOperation = Connector.SingleInsert(context);
            ISingleInsertResultInternal Result = ActiveOperation.Result;

            if (Result.IsStarted)
            {
                ActiveOperationTable.Add(ActiveOperation);
                NewOperationEvent.Set();
                Result.WaitCompleted();
            }

            return Result;
        }
        #endregion

        #region Multi Insert
        /// <summary>
        ///     Executes a request to insert several values in a table.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public IMultiInsertResult Run(IMultiInsertContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<IMultiInsertResult> RunAsync(IMultiInsertContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => Execute(context));
        }

        /// <summary>
        ///     Executes a request to insert several values in a table.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        protected virtual IMultiInsertResult Execute(IMultiInsertContext context)
        {
            if (!IsOperationThreadStarted)
                return new MultiInsertResult(false);

            if (context.RowCount == 0 || context.EntryList.Count == 0)
                return new MultiInsertResult(true);

            IActiveOperation<IMultiInsertResultInternal> ActiveOperation = Connector.MultiInsert(context);
            IMultiInsertResultInternal Result = ActiveOperation.Result;

            if (Result.IsStarted)
            {
                ActiveOperationTable.Add(ActiveOperation);
                NewOperationEvent.Set();
                Result.WaitCompleted();
            }

            return Result;
        }
        #endregion

        #region Single Row Delete
        /// <summary>
        ///     Executes a request to delete rows in a table, with constraints on some values.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public ISingleRowDeleteResult Run(ISingleRowDeleteContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<ISingleRowDeleteResult> RunAsync(ISingleRowDeleteContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => Execute(context));
        }

        /// <summary>
        ///     Executes a request to delete rows in a table, with constraints on some values.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        protected virtual ISingleRowDeleteResult Execute(ISingleRowDeleteContext context)
        {
            if (!IsOperationThreadStarted)
                return new SingleRowDeleteResult(false);

            IActiveOperation<ISingleRowDeleteResultInternal> ActiveOperation = Connector.SingleRowDelete(context);
            ISingleRowDeleteResultInternal Result = ActiveOperation.Result;

            if (Result.IsStarted)
            {
                ActiveOperationTable.Add(ActiveOperation);
                NewOperationEvent.Set();
                Result.WaitCompleted();
            }

            return Result;
        }
        #endregion

        #region Multi Row Delete
        /// <summary>
        ///     Executes a request to delete rows in a table, with constraints.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public IMultiRowDeleteResult Run(IMultiRowDeleteContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<IMultiRowDeleteResult> RunAsync(IMultiRowDeleteContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => Execute(context));
        }

        /// <summary>
        ///     Executes a request to delete rows in a table, with constraints.
        ///     This is the synchronous implementation.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        protected virtual IMultiRowDeleteResult Execute(IMultiRowDeleteContext context)
        {
            if (!IsOperationThreadStarted)
                return new MultiRowDeleteResult(false);

            IActiveOperation<IMultiRowDeleteResultInternal> ActiveOperation = Connector.MultiRowDelete(context);
            IMultiRowDeleteResultInternal Result = ActiveOperation.Result;

            if (Result.IsStarted)
            {
                ActiveOperationTable.Add(ActiveOperation);
                NewOperationEvent.Set();
                Result.WaitCompleted();
            }

            return Result;
        }
        #endregion

        #region Close
        /// <summary>
        ///     Closes a database opened with the <see cref="Open"/> method.
        /// </summary>
        public void Close()
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            ExecuteClose();
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task CloseAsync()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Task.Run(() => ExecuteClose());
        }

        /// <summary>
        ///     Closes a database opened with the <see cref="Open"/> method.
        ///     This is the synchronous implementation.
        /// </summary>
        protected virtual void ExecuteClose()
        {
            StopOperationThread();

            Connector.Close();
            Debug.Assert(!Connector.IsOpen);
        }
        #endregion

        #region Operations
        private void StartOperationThread()
        {
            try
            {
                ShutdownEvent = new ManualResetEvent(false);
                NewOperationEvent = new AutoResetEvent(false);
                ActiveOperationTable = new List<IActiveOperation>();

                OperationThread = new Thread(new ThreadStart(ExecuteOperation));
                OperationThread.Start();
            }
            catch (ApplicationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Debugging.PrintExceptionMessage(e.Message);
                OperationThread = null;
            }
        }

        private bool IsOperationThreadStarted { get { return OperationThread != null; } }

        private void ExecuteOperation()
        {
            WaitHandle[] ThreadHandles = new WaitHandle[] { NewOperationEvent, ShutdownEvent};
            bool Exit = false;

            while (!Exit)
            {
                int ThreadWaitResult = WaitHandle.WaitAny(ThreadHandles, Timeout.InfiniteTimeSpan);

                if (ThreadWaitResult == 0)
                {
                    IActiveOperation[] ActiveOperations = new IActiveOperation[ActiveOperationTable.Count];
                    WaitHandle[] OperationHandles = new WaitHandle[ActiveOperationTable.Count + 1];

                    OperationHandles[0] = ShutdownEvent;

                    int n = 0;
                    foreach (IActiveOperation ActiveOperation in ActiveOperationTable)
                    {
                        ActiveOperations[n] = ActiveOperation;
                        OperationHandles[n + 1] = ActiveOperation.ResultBase.AsyncResult.AsyncWaitHandle;
                        n++;
                    }

                    int OperationWaitResult = WaitHandle.WaitAny(OperationHandles, TimeSpan.FromSeconds(1.0));

                    if (OperationWaitResult == WaitHandle.WaitTimeout)
                        NewOperationEvent.Set();

                    else if (OperationWaitResult > 0 && OperationWaitResult <= OperationHandles.Length)
                    {
                        IActiveOperation ActiveOperation = ActiveOperations[OperationWaitResult - 1];
                        ActiveOperationTable.Remove(ActiveOperation);

                        Connector.NotifyOperationCompleted(ActiveOperation);
                    }
                    else
                        Exit = true;
                }

                else
                    Exit = true;
            }
        }

        private void StopOperationThread()
        {
            ShutdownEvent.Set();
            OperationThread.Join();
            OperationThread = null;
        }

        private Thread OperationThread;
        private AutoResetEvent NewOperationEvent;
        private ManualResetEvent ShutdownEvent;
        private IList<IActiveOperation> ActiveOperationTable;
        #endregion

        #region Implementation of IDisposable
        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="SimpleDatabase"/> and optionally releases the managed resources.
        /// </summary>
        /// <parameters>
        /// <param name="IsDisposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// </parameters>
        protected virtual void Dispose(bool IsDisposing)
        {
            if (IsDisposing)
                DisposeNow();
        }

        private void DisposeNow()
        {
            if (IsOpen)
                ExecuteClose();

            if (ConnectionMutex != null)
            {
                ConnectionMutex.Dispose();
                ConnectionMutex = null;
            }
        }

        /// <summary>
        ///     Releases the resources used by the <see cref="SimpleDatabase"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~SimpleDatabase()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Dispose(false);
        }
        #endregion
    }
}
