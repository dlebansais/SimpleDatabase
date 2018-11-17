using System;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to insert several values in a table.
    /// </summary>
    public interface IMultiInsertResult : IInsertResult, IModifyResult
    {
    }
}

namespace Database.Internal
{
    internal interface IMultiInsertResultInternal : IInsertResultInternal, IModifyResultInternal, IMultiInsertResult
    {
    }

    internal class MultiInsertResult : InsertResult, IMultiInsertResultInternal
    {
        public MultiInsertResult(IMultiInsertOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public MultiInsertResult(bool success)
            : base(success)
        {
        }
    }
}
