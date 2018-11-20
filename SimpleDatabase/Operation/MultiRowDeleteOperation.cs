namespace Database.Internal
{
    internal interface IDeleteOperation : IOperation<IDeleteContext, IDeleteResult>, IModifyOperation
    {
    }

    internal class DeleteOperation : Operation<IDeleteContext, IDeleteResult>, IDeleteOperation
    {
        #region Init
        public DeleteOperation(IDeleteContext context)
            : base(context)
        {
        }
        #endregion
    }
}
