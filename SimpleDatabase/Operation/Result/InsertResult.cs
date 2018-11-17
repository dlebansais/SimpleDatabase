using System;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request that inserts values in a table.
    /// </summary>
    public interface IInsertResult : IModifyResult
    {
    }
}

namespace Database.Internal
{
    internal interface IInsertResultInternal : IModifyResultInternal, IInsertResult
    {
    }

    internal abstract class InsertResult : Result, IInsertResultInternal
    {
        public InsertResult(IInsertOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public InsertResult(bool success)
            : base(success)
        {
        }
    }
}
