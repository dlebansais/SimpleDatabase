namespace Database.Internal
{
    internal interface IQueryOperation : IOperation
    {
    }

    internal interface IQueryOperation<TContext, TResult> : IOperation<TContext, TResult>
        where TContext : IContext
        where TResult : IResult
    {
    }

    internal abstract class QueryOperation<TContext, TResult> : Operation<TContext, TResult>, IQueryOperation<TContext, TResult>, IQueryOperation
        where TContext : IContext
        where TResult : IResult
    {
        #region Init
        public QueryOperation(TContext context)
            : base(context)
        {
        }
        #endregion
    }
}
