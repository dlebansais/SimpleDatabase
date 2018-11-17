using System;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to delete rows in a table, with constraints on some values.
    /// </summary>
    public interface ISingleRowDeleteResult : IModifyResult
    {
    }
}

namespace Database.Internal
{
    internal interface ISingleRowDeleteResultInternal : IModifyResultInternal, ISingleRowDeleteResult
    {
    }

    internal class SingleRowDeleteResult : Result, ISingleRowDeleteResultInternal
    {
        public SingleRowDeleteResult(ISingleRowDeleteOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public SingleRowDeleteResult(bool success)
            : base(success)
        {
        }
    }
}
