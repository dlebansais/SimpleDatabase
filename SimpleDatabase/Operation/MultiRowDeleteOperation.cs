namespace Database.Internal
{
    internal interface IMultiRowDeleteOperation : IOperation<IMultiRowDeleteContext, IMultiRowDeleteResult>, IModifyOperation
    {
    }

    internal class MultiRowDeleteOperation : Operation<IMultiRowDeleteContext, IMultiRowDeleteResult>, IMultiRowDeleteOperation
    {
        #region Init
        public MultiRowDeleteOperation(IMultiRowDeleteContext context)
            : base(context)
        {
        }
        #endregion
    }
}
