using System.Collections.Generic;

namespace Database.Internal
{
    internal interface IUpdateOperation : IOperation<IUpdateContext, IUpdateResult>, IModifyOperation
    {
        IReadOnlyCollection<IColumnValuePair<byte[]>> GetDataEntryList();
    }

    internal class UpdateOperation : Operation<IUpdateContext, IUpdateResult>, IUpdateOperation
    {
        #region Init
        public UpdateOperation(IUpdateContext context)
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
