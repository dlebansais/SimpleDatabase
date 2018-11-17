using System.Collections.Generic;

namespace Database.Internal
{
    internal interface ISingleInsertOperation : IInsertOperation<ISingleInsertContext, ISingleInsertResult>, IInsertOperation
    {
        IReadOnlyCollection<IColumnValuePair<byte[]>> GetDataEntryList();
    }

    internal class SingleInsertOperation : InsertOperation<ISingleInsertContext, ISingleInsertResult>, ISingleInsertOperation
    {
        #region Init
        public SingleInsertOperation(ISingleInsertContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual IReadOnlyCollection<IColumnValuePair<byte[]>> GetDataEntryList()
        {
            List<IColumnValuePair<byte[]>> Result = new List<IColumnValuePair<byte[]>>();
            foreach (IColumnValuePair Entry in Context.EntryList)
                if (Entry is IColumnValuePair<byte[]> AsDataEntry)
                    Result.Add(AsDataEntry);

            return Result;
        }
        #endregion
    }
}
