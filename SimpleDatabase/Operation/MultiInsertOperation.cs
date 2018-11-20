using System.Collections.Generic;

namespace Database.Internal
{
    internal interface IMultiInsertOperation : IOperation<IMultiInsertContext, IMultiInsertResult>, IModifyOperation
    {
        IReadOnlyCollection<IColumnValueCollectionPair<byte[]>> GetDataEntryList();
    }

    internal class MultiInsertOperation : Operation<IMultiInsertContext, IMultiInsertResult>, IMultiInsertOperation
    {
        #region Init
        public MultiInsertOperation(IMultiInsertContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual IReadOnlyCollection<IColumnValueCollectionPair<byte[]>> GetDataEntryList()
        {
            List<IColumnValueCollectionPair<byte[]>> Result = new List<IColumnValueCollectionPair<byte[]>>();
            foreach (IColumnValueCollectionPair Entry in Context.EntryList)
                if (Entry is IColumnValueCollectionPair<byte[]> AsDataEntry)
                    Result.Add(AsDataEntry);

            return Result;
        }
        #endregion
    }
}
