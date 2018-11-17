namespace Database.Internal
{
    internal interface IInsertOperation : IOperation, IModifyOperation
    {
    }

    internal interface IInsertOperation<TContext, TResult> : IOperation<TContext, TResult>, IModifyOperation
        where TContext : IInsertContext
        where TResult : IInsertResult
    {
    }

    internal abstract class InsertOperation<TContext, TResult> : Operation<TContext, TResult>, IInsertOperation<TContext, TResult>, IInsertOperation
        where TContext : IInsertContext
        where TResult : IInsertResult
    {
        #region Init
        public InsertOperation(TContext context)
            : base(context)
        {
        }
        #endregion
    }
}
