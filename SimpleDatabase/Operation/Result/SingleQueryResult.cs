using System;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request that queries values on a single table, with constraints.
    /// </summary>
    public interface ISingleQueryResult : IQueryResult
    {
    }
}

namespace Database.Internal
{
    internal interface ISingleQueryResultInternal : IQueryResultInternal, ISingleQueryResult
    {
    }

    internal class SingleQueryResult : QueryResult, ISingleQueryResultInternal
    {
        public SingleQueryResult(ISingleQueryOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public SingleQueryResult(bool success)
            : base(success)
        {
        }
    }
}
