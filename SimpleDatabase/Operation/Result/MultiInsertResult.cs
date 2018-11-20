using System;
using System.Diagnostics;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to insert several values in a table.
    /// </summary>
    public interface IMultiInsertResult : IModifyResult, IDataResult
    {
        /// <summary>
        ///     Gets the primary key column and value of the last inserted row.
        /// </summary>
        /// <returns>
        ///     The primary key column and value of the last inserted row.
        /// </returns>
        IColumnValuePair LastCreatedKeyId { get; }
    }
}

namespace Database.Internal
{
    internal interface IMultiInsertResultInternal : IMultiInsertResult, IModifyResultInternal, IDataResultInternal
    {
        void SetCompletedWithId(IColumnValuePair lastCreatedKeyId);
    }

    internal class MultiInsertResult : Result, IMultiInsertResultInternal
    {
        #region Init
        public MultiInsertResult(IMultiInsertOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public MultiInsertResult(bool success)
            : base(success)
        {
        }
        #endregion

        #region Properties
        public IColumnValuePair LastCreatedKeyId { get; private set; }
        #endregion

        #region Descendant Interface
        public void SetCompletedWithId(IColumnValuePair lastCreatedKeyId)
        {
            LastCreatedKeyId = lastCreatedKeyId;
            base.SetCompleted(true);
        }

        public override void SetCompleted(bool success)
        {
            Debug.Assert(!success);
            base.SetCompleted(false);
        }
        #endregion
    }
}
