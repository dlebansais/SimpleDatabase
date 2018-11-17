﻿using System;

namespace Database.Internal
{
    internal interface IDatabaseConnector
    {
        bool IsCredentialValid(ICredential credential, TimeSpan minDuration);
        bool CreateCredential(string rootId, string rootPassword, ICredential credential);
        bool CreateTables(ICredential credential, ISchemaDescriptor schema);

        bool Open(ICredential credential);
        bool IsOpen { get; }
        void Close();

        IActiveOperation<ISingleQueryResultInternal> SingleQuery(ISingleQueryContext context);
        IActiveOperation<IMultiQueryResultInternal> MultiQuery(IMultiQueryContext context);
        IActiveOperation<IUpdateResultInternal> Update(IUpdateContext context);
        IActiveOperation<ISingleInsertResultInternal> SingleInsert(ISingleInsertContext context);
        IActiveOperation<IMultiInsertResultInternal> MultiInsert(IMultiInsertContext context);
        IActiveOperation<ISingleRowDeleteResultInternal> SingleRowDelete(ISingleRowDeleteContext context);
        IActiveOperation<IMultiRowDeleteResultInternal> MultiRowDelete(IMultiRowDeleteContext context);
        void NotifyOperationCompleted(IActiveOperation activeOperation);
    }

    internal abstract class DatabaseConnector : IDatabaseConnector
    {
        public abstract bool IsCredentialValid(ICredential credential, TimeSpan minDuration);
        public abstract bool CreateCredential(string rootId, string rootPassword, ICredential credential);
        public abstract bool CreateTables(ICredential credential, ISchemaDescriptor schema);

        public abstract bool IsOpen { get; }
        public abstract bool Open(ICredential credential);
        public abstract void Close();

        public abstract IActiveOperation<ISingleQueryResultInternal> SingleQuery(ISingleQueryContext context);
        public abstract IActiveOperation<IMultiQueryResultInternal> MultiQuery(IMultiQueryContext context);
        public abstract IActiveOperation<IUpdateResultInternal> Update(IUpdateContext context);
        public abstract IActiveOperation<ISingleInsertResultInternal> SingleInsert(ISingleInsertContext context);
        public abstract IActiveOperation<IMultiInsertResultInternal> MultiInsert(IMultiInsertContext context);
        public abstract IActiveOperation<ISingleRowDeleteResultInternal> SingleRowDelete(ISingleRowDeleteContext context);
        public abstract IActiveOperation<IMultiRowDeleteResultInternal> MultiRowDelete(IMultiRowDeleteContext context);
        public abstract void NotifyOperationCompleted(IActiveOperation activeOperation);
    }
}