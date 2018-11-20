using System;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to delete rows in a table, with constraints.
    /// </summary>
    public interface IDeleteResult : IModifyResult
    {
    }
}

namespace Database.Internal
{
    internal interface IDeleteResultInternal : IModifyResultInternal, IDeleteResult
    {
    }

    internal class DeleteResult : Result, IDeleteResultInternal
    {
        public DeleteResult(IDeleteOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public DeleteResult(bool success)
            : base(success)
        {
        }
    }
}
