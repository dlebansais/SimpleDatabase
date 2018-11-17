using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to query values from several tables.
    /// </summary>
    public interface IMultiQueryResult : IQueryResult
    {
    }
}

namespace Database.Internal
{
    internal interface IMultiQueryResultInternal : IQueryResultInternal, IMultiQueryResult
    {
        void SetCompletedWithResult(bool success, List<IResultRow> Rows);
    }

    internal class MultiQueryResult : QueryResult, IMultiQueryResultInternal
    {
        public MultiQueryResult(IMultiQueryOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public MultiQueryResult(bool success)
            : base(success)
        {
        }
    }
}
