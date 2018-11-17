using System;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to update values in a row of a table, with constraints on the previous values.
    /// </summary>
    public interface IUpdateResult : IModifyResult, IDataResult
    {
    }
}

namespace Database.Internal
{
    internal interface IUpdateResultInternal : IModifyResultInternal, IDataResultInternal, IUpdateResult
    {
    }

    internal class UpdateResult : Result, IUpdateResultInternal
    {
        public UpdateResult(IUpdateOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public UpdateResult(bool success)
            : base(success)
        {
        }
    }
}
