namespace Database.Internal
{
    internal interface IJoinQueryOperation : IQueryOperation<IJoinQueryContext, IJoinQueryResult>, IQueryOperation
    {
    }

    internal class JoinQueryOperation : QueryOperation<IJoinQueryContext, IJoinQueryResult>, IJoinQueryOperation
    {
        #region Init
        public JoinQueryOperation(IJoinQueryContext context)
            : base(context)
        {
        }
        #endregion
    }
}
