using System;
using System.Diagnostics;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents the result of a request to delete rows in a table, with constraints.
    /// </summary>
    public interface IDeleteResult : IModifyResult
    {
        /// <summary>
        ///     Gets the number of deleted rows.
        /// </summary>
        /// <returns>
        ///     The number of deleted rows.
        /// </returns>
        int DeletedRowCount { get; }
    }
    #endregion
}

namespace Database.Internal
{
    internal interface IDeleteResultInternal : IModifyResultInternal, IDeleteResult
    {
        void SetCompletedWithCount(int deletedRowCount);
    }

    internal class DeleteResult : Result, IDeleteResultInternal
    {
        #region Init
        public DeleteResult(IDeleteOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
            DeletedRowCount = -1;
        }

        public DeleteResult(bool success)
            : base(success)
        {
            DeletedRowCount = -1;
        }
        #endregion

        #region Properties
        public int DeletedRowCount { get; private set; }
        #endregion

        #region Descendant Interface
        public void SetCompletedWithCount(int deletedRowCount)
        {
            DeletedRowCount = deletedRowCount;
            base.SetCompleted(true);
        }

        public override void SetCompleted(bool success)
        {
            Debug.Assert(!success);
            base.SetCompleted(false);
        }
        #endregion

        #region Debugging
        public override string ToString()
        {
            if (Success)
                return $"Success, {DeletedRowCount} row(s) deleted";
            else if (IsCompleted)
                return $"Failed";
            else
                return $"...";
        }
        #endregion
    }
}
