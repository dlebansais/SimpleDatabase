namespace Database
{
    /// <summary>
    ///     Represents the result of a request that manipulates values of byte[] type.
    /// </summary>
    public interface IDataResult : IResult
    {
    }
}

namespace Database.Internal
{
    internal interface IDataResultInternal : IResultInternal, IDataResult
    {
    }
}
