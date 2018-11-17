namespace Database.Internal
{
    internal interface IOperation
    {
        IContext Context { get; }
    }

    internal interface IOperation<TContext, TResult>
        where TContext : IContext
        where TResult : IResult
    {
        TContext Context { get; }
    }

    internal class Operation<TContext, TResult> : IOperation<TContext, TResult>, IOperation
        where TContext : IContext
        where TResult : IResult
    {
        #region Init
        public Operation(TContext context)
        {
            Context = context;
        }
        #endregion

        #region Properties
        public TContext Context { get; }
        IContext IOperation.Context { get { return Context; } }
        #endregion
    }
}
