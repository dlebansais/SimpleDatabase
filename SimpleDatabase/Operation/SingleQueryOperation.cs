namespace Database.Internal
{
    internal interface ISingleQueryOperation : IQueryOperation<ISingleQueryContext, ISingleQueryResult>, IQueryOperation
    {
    }

    internal class SingleQueryOperation : QueryOperation<ISingleQueryContext, ISingleQueryResult>, ISingleQueryOperation
    {
        #region Init
        public SingleQueryOperation(ISingleQueryContext context)
            : base(context)
        {
        }
        #endregion
    }
}
