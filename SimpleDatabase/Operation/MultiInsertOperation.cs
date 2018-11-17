namespace Database.Internal
{
    internal interface IMultiInsertOperation : IInsertOperation<IMultiInsertContext, IMultiInsertResult>, IInsertOperation
    {
    }

    internal class MultiInsertOperation : InsertOperation<IMultiInsertContext, IMultiInsertResult>, IMultiInsertOperation
    {
        #region Init
        public MultiInsertOperation(IMultiInsertContext context)
            : base(context)
        {
        }
        #endregion
    }
}
