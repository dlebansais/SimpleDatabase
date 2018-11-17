namespace Database.Internal
{
    internal interface IMultiQueryOperation : IQueryOperation<IMultiQueryContext, IMultiQueryResult>, IQueryOperation
    {
    }

    internal class MultiQueryOperation : QueryOperation<IMultiQueryContext, IMultiQueryResult>, IMultiQueryOperation
    {
        #region Init
        public MultiQueryOperation(IMultiQueryContext context)
            : base(context)
        {
        }
        #endregion
    }
}
