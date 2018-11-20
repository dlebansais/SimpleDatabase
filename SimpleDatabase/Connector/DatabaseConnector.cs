﻿using System;

namespace Database.Internal
{
    internal interface IDatabaseConnector
    {
        bool IsCredentialValid(ICredential credential, TimeSpan minDuration);
        bool CreateCredential(string rootId, string rootPassword, ICredential credential);
        bool CreateTables(ICredential credential);
        void DeleteTables(ICredential credential);
        void DeleteCredential(string rootId, string rootPassword, ICredential credential);

        bool Open(ICredential credential);
        bool IsOpen { get; }
        void Close();

        IActiveOperation<ISingleQueryResultInternal> SingleQuery(ISingleQueryContext context);
        IActiveOperation<IMultiQueryResultInternal> MultiQuery(IMultiQueryContext context);
        IActiveOperation<IUpdateResultInternal> Update(IUpdateContext context);
        IActiveOperation<IInsertResultInternal> Insert(IInsertContext context);
        IActiveOperation<IDeleteResultInternal> Delete(IDeleteContext context);
        void NotifyOperationCompleted(IActiveOperation activeOperation);

        int LastErrorCode { get; }
    }

    internal abstract class DatabaseConnector : IDatabaseConnector
    {
        public abstract bool IsCredentialValid(ICredential credential, TimeSpan minDuration);
        public abstract bool CreateCredential(string rootId, string rootPassword, ICredential credential);
        public abstract bool CreateTables(ICredential credential);
        public abstract void DeleteTables(ICredential credential);
        public abstract void DeleteCredential(string rootId, string rootPassword, ICredential credential);

        public abstract bool IsOpen { get; }
        public abstract bool Open(ICredential credential);
        public abstract void Close();

        public abstract IActiveOperation<ISingleQueryResultInternal> SingleQuery(ISingleQueryContext context);
        public abstract IActiveOperation<IMultiQueryResultInternal> MultiQuery(IMultiQueryContext context);
        public abstract IActiveOperation<IUpdateResultInternal> Update(IUpdateContext context);
        public abstract IActiveOperation<IInsertResultInternal> Insert(IInsertContext context);
        public abstract IActiveOperation<IDeleteResultInternal> Delete(IDeleteContext context);
        public abstract void NotifyOperationCompleted(IActiveOperation activeOperation);

        public abstract int LastErrorCode { get; }
        public abstract int IgnoreErrorCode { get; set; }
    }
}
