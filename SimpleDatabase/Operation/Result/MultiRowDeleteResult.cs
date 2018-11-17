using System;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to delete rows in a table, with constraints.
    /// </summary>
    public interface IMultiRowDeleteResult : IModifyResult
    {
    }
}

namespace Database.Internal
{
    internal interface IMultiRowDeleteResultInternal : IModifyResultInternal, IMultiRowDeleteResult
    {
    }

    internal class MultiRowDeleteResult : Result, IMultiRowDeleteResultInternal
    {
        public MultiRowDeleteResult(IMultiRowDeleteOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public MultiRowDeleteResult(bool success)
            : base(success)
        {
        }
    }
}
