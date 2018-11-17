namespace Database.Internal
{
    internal interface IActiveOperation
    {
        IResultInternal ResultBase { get; }
    }

    internal interface IActiveOperation<TInternal> : IActiveOperation
        where TInternal : IResultInternal
    {
        TInternal Result { get; }
    }

    internal class ActiveOperation<TInternal> : IActiveOperation<TInternal>, IActiveOperation
        where TInternal : IResultInternal
    {
        public ActiveOperation(TInternal result)
        {
            Result = result;
        }

        public TInternal Result { get; }
        IResultInternal IActiveOperation.ResultBase { get { return Result; } }
    }
}
