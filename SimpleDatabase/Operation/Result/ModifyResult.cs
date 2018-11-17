namespace Database
{
    /// <summary>
    ///     Represents the result of a request that add, delete or modify values in a table.
    /// </summary>
    public interface IModifyResult : IResult
    {
    }
}

namespace Database.Internal
{
    internal interface IModifyResultInternal : IResultInternal, IModifyResult
    {
    }
}
