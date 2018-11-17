namespace Database.Internal
{
    internal interface ISingleRowDeleteOperation : IOperation<ISingleRowDeleteContext, ISingleRowDeleteResult>, IModifyOperation
    {
    }

    internal class SingleRowDeleteOperation : Operation<ISingleRowDeleteContext, ISingleRowDeleteResult>, ISingleRowDeleteOperation
    {
        #region Init
        public SingleRowDeleteOperation(ISingleRowDeleteContext context)
            : base(context)
        {
        }
        #endregion
    }
}
