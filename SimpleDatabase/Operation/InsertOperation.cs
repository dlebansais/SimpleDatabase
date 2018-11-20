using System.Collections.Generic;

namespace Database.Internal
{
    internal interface IInsertOperation : IOperation<IInsertContext, IInsertResult>, IModifyOperation
    {
        IReadOnlyCollection<IColumnValueCollectionPair<byte[]>> GetDataEntryList();
    }

    internal class InsertOperation : Operation<IInsertContext, IInsertResult>, IInsertOperation
    {
        #region Init
        public InsertOperation(IInsertContext context)
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
