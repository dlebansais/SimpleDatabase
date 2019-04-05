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
        ///     Initializes the database, loading the engine associated to <paramref name="connectorType"/>.
        /// </summary>
        /// <parameters>
        /// <param name="connectorType">The engine backing the database.</param>
        /// <param name="connectionOption">Options related to connection to a server.</param>
        /// <param name="isDebugTraceEnabled">True to turn traces on.</param>
        /// </parameters>
        void Initialize(ConnectorType connectorType, ConnectionOption connectionOption, bool isDebugTraceEnabled);

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
        ///     Indicates if debug traces are ON or OFF
        /// </summary>
        /// <returns>
        ///     True if debug traces are ON.
        /// </returns>
        bool IsDebugTraceEnabled { get; }

        /// <summary>
        ///     Gets the last error code returned by the database engine.
        /// </summary>
        /// <returns>
        ///     The last error code returned by the database engine.
        /// </returns>
        int LastErrorCode { get; }

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
        ///     Indicates if the database server is started.
        /// </summary>
        /// <returns>
        ///     True if the database server is started. False otherwise.
        /// </returns>
        bool IsServerStarted { get; }

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
        IJoinQueryResult Run(IJoinQueryContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<IJoinQueryResult> RunAsync(IJoinQueryContext context);
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
        ///     Executes a request to insert several values in a table.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        IInsertResult Run(IInsertContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<IInsertResult> RunAsync(IInsertContext context);
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
        IDeleteResult Run(IDeleteContext context);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Task<IDeleteResult> RunAsync(IDeleteContext context);
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
            Initialize(connectorType, connectionOption, true);
        }

        /// <summary>
        ///     Initializes the database, loading the engine associated to <paramref name="connectorType"/>.
        /// </summary>
        /// <parameters>
        /// <param name="connectorType">The engine backing the database.</param>
        /// <param name="connectionOption">Options related to connection to a server.</param>
        /// <param name="isDebugTraceEnabled">True to turn traces on.</param>
        /// </parameters>
        public virtual void Initialize(ConnectorType connectorType, ConnectionOption connectionOption, bool isDebugTraceEnabled)
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
                        throw new ArgumentOutOfRangeException(nameof(connectorType));
                }

                IsDebugTraceEnabled = isDebugTraceEnabled;
                if (IsDebugTraceEnabled)
                    Debugging.Print("Connector created: " + Connector.ToString());
            }
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (Exception e)
            {
                if (IsDebugTraceEnabled)
                    Debugging.PrintExceptionMessage(e.Message);
            }
        }

        private IDatabaseConnector Connector;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the engine backing the database.
        /// </summary>
        /// <returns>
        ///     The engine backing the database.
        /// </returns>
        public ConnectorType ConnectorType { get; private set; }

        /// <summary>
        ///     Gets the options related to connection to a server
        /// </summary>
        /// <returns>
        ///     The options related to connection to a server
        /// </returns>
        public ConnectionOption ConnectionOption { get; private set; }

        /// <summary>
        ///     Indicates if debug traces are ON or OFF
        /// </summary>
        /// <returns>
        ///     True if debug traces are ON.
        /// </returns>
        public bool IsDebugTraceEnabled
        {
            get { return Connector != null ? Connector.IsDebugTraceEnabled : false; }
            private set
            {
                if (Connector != null)
                    Connector.IsDebugTraceEnabled = value;
            }
        }

        /// <summary>
        ///     Gets the last error code returned by the database engine.
        /// </summary>
        /// <returns>
        ///     The last error code returned by the database engine.
        /// </returns>
        public int LastErrorCode { get { return Connector != null ? Connector.LastErrorCode : 0; } }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public int IgnoreErrorCode { get { return Connector != null ? ((DatabaseConnector)Connector).IgnoreErrorCode : 0; } set { if (Connector != null) ((DatabaseConnector)Connector).IgnoreErrorCode = value; } }
        public bool CanIntBeNULL { get { return Connector != null ? ((DatabaseConnector)Connector).CanIntBeNULL : false; } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
        ///     Indicates if the database server is started.
        /// </summary>
        /// <returns>
        ///     True if the database server is started. False otherwise.
        /// </returns>
        public virtual bool IsServerStarted
        {
            get { return Connector != null && Connector.IsServerStarted; }
        }

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

            if (Connector.IsOpen)
                return false;

            return ExecuteOpen(credential);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual Task<bool> OpenAsync(ICredential credential)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Debug.Assert(Connector != null);
            Debug.Assert(!Connector.IsOpen);

            if (Connector.IsOpen)
                return Task.Run(() => false);

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
            bool Success;
            if (Connector.Open(credential))
            {
                Debug.Assert(Connector.IsOpen);

                StartOperationThread();
                Success = true;
            }
            else
                Success = false;

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
                return new SingleQueryResult(false, ResultError.ErrorFatalNoOperationThread);

            IActiveOperation<ISingleQueryResultInternal> ActiveOperation = Connector.SingleQuery(context);
            ISingleQueryResultInternal Result = ActiveOperation.Result;
            FinalizeOrQueue(ActiveOperation);

            return Result;
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
        public IJoinQueryResult Run(IJoinQueryContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<IJoinQueryResult> RunAsync(IJoinQueryContext context)
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
        protected virtual IJoinQueryResult Execute(IJoinQueryContext context)
        {
            if (!IsOperationThreadStarted)
                return new JoinQueryResult(false, ResultError.ErrorFatalNoOperationThread);

            IActiveOperation<IJoinQueryResultInternal> ActiveOperation = Connector.JoinQuery(context);
            IJoinQueryResultInternal Result = ActiveOperation.Result;
            FinalizeOrQueue(ActiveOperation);

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
                return new UpdateResult(false, ResultError.ErrorFatalNoOperationThread);

            IActiveOperation<IUpdateResultInternal> ActiveOperation = Connector.Update(context);
            IUpdateResultInternal Result = ActiveOperation.Result;
            FinalizeOrQueue(ActiveOperation);

            return Result;
        }
        #endregion

        #region Insert
        /// <summary>
        ///     Executes a request to insert several values in a table.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public IInsertResult Run(IInsertContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<IInsertResult> RunAsync(IInsertContext context)
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
        protected virtual IInsertResult Execute(IInsertContext context)
        {
            if (!IsOperationThreadStarted)
                return new InsertResult(false, ResultError.ErrorFatalNoOperationThread);

            if (context.RowCount == 0 || context.EntryList.Count == 0)
                return new InsertResult(true, ResultError.ErrorNone);

            IActiveOperation<IInsertResultInternal> ActiveOperation = Connector.Insert(context);
            IInsertResultInternal Result = ActiveOperation.Result;
            FinalizeOrQueue(ActiveOperation);

            return Result;
        }
        #endregion

        #region Delete
        /// <summary>
        ///     Executes a request to delete rows in a table, with constraints.
        /// </summary>
        /// <parameters>
        /// <param name="context">Description of the request.</param>
        /// </parameters>
        /// <returns>
        ///     The request result.
        /// </returns>
        public IDeleteResult Run(IDeleteContext context)
        {
            Debug.Assert(Connector != null);
            Debug.Assert(Connector.IsOpen);

            return Execute(context);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Task<IDeleteResult> RunAsync(IDeleteContext context)
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
        protected virtual IDeleteResult Execute(IDeleteContext context)
        {
            if (!IsOperationThreadStarted)
                return new DeleteResult(false, ResultError.ErrorFatalNoOperationThread);

            IActiveOperation<IDeleteResultInternal> ActiveOperation = Connector.Delete(context);
            IDeleteResultInternal Result = ActiveOperation.Result;
            FinalizeOrQueue(ActiveOperation);

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
#if TRACE
            catch (ApplicationException)
            {
                throw;
            }
#endif
            catch (Exception e)
            {
                if (IsDebugTraceEnabled)
                    Debugging.PrintExceptionMessage(e.Message);
                OperationThread = null;
            }
        }

        private bool IsOperationThreadStarted
        {
            get
            {
                if (LastCompletionException != null)
                {
                    Exception e = LastCompletionException;
                    LastCompletionException = null;

#if DEBUG
#elif TRACE
                    throw e;
#endif
                }

                return OperationThread != null;
            }
        }

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

                        Connector.NotifyOperationCompleted(ActiveOperation, out Exception CompletionException);
                        if (LastCompletionException == null && CompletionException != null)
                            LastCompletionException = CompletionException;
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

        private void FinalizeOrQueue(IActiveOperation activeOperation)
        {
            IResultInternal Result = activeOperation.ResultBase;
            if (Result.IsStarted)
            {
                Result.CheckIfCompletedSynchronously(out bool IsCompletedSynchronously);

                if (IsCompletedSynchronously)
                {
                    Connector.NotifyOperationCompleted(activeOperation, out Exception CompletionException);
                    if (CompletionException != null)
                        throw CompletionException;
                }
                else
                {
                    ActiveOperationTable.Add(activeOperation);
                    NewOperationEvent.Set();
                    Result.WaitCompleted();
                }
            }
        }

        /// <summary>
        /// Returns the current instance as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Connector != null ? Connector.ToString() : "<No connector>";
        }

        private Thread OperationThread;
        private AutoResetEvent NewOperationEvent;
        private ManualResetEvent ShutdownEvent;
        private IList<IActiveOperation> ActiveOperationTable;
        private Exception LastCompletionException;
        #endregion

        #region Implementation of IDisposable
        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="SimpleDatabase"/> and optionally releases the managed resources.
        /// </summary>
        /// <parameters>
        /// <param name="isDisposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// </parameters>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
                DisposeNow();
        }

        private void DisposeNow()
        {
            if (IsOpen)
                ExecuteClose();
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
