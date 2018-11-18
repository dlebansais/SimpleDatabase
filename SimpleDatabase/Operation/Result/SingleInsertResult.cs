using System;
using System.Diagnostics;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to insert a single row of values in a table.
    /// </summary>
    public interface ISingleInsertResult : IInsertResult, IDataResult
    {
        /// <summary>
        ///     Gets the primary key column and value of the inserted row.
        /// </summary>
        /// <returns>
        ///     The primary key column and value of the inserted row.
        /// </returns>
        IColumnValuePair CreatedKeyId { get; }
    }
}

namespace Database.Internal
{
    internal interface ISingleInsertResultInternal : IInsertResultInternal, IDataResultInternal, ISingleInsertResult
    {
        void SetCompletedWithId(bool success, IColumnValuePair createdKeyId);
    }

    internal class SingleInsertResult : InsertResult, ISingleInsertResultInternal
    {
        public SingleInsertResult(ISingleInsertOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
            CreatedKeyId = null;
        }

        public SingleInsertResult(bool success)
            : base(success)
        {
            CreatedKeyId = null;
        }

        #region Properties
        public IColumnValuePair CreatedKeyId { get; private set; }
        #endregion

        #region Descendant Interface
        public void SetCompletedWithId(bool success, IColumnValuePair createdKeyId)
        {
            if (success)
                CreatedKeyId = createdKeyId;

            base.SetCompleted(success);
        }

        public override void SetCompleted(bool success)
        {
            Debug.Assert(!success);
            base.SetCompleted(success);
        }
        #endregion
    }
}
