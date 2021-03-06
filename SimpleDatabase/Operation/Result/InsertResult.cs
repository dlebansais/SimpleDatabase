﻿using System;
using System.Diagnostics;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents the result of a request to insert several values in a table.
    /// </summary>
    public interface IInsertResult : IModifyResult, IDataResult
    {
        /// <summary>
        ///     Gets the primary key column and value of the last inserted row.
        /// </summary>
        /// <returns>
        ///     The primary key column and value of the last inserted row.
        /// </returns>
        IColumnValuePair LastCreatedKeyId { get; }
    }
    #endregion
}

namespace Database.Internal
{
    internal interface IInsertResultInternal : IInsertResult, IModifyResultInternal, IDataResultInternal
    {
        void SetCompletedWithId(IColumnValuePair lastCreatedKeyId);
    }

    internal class InsertResult : Result, IInsertResultInternal
    {
        #region Init
        public InsertResult(IInsertOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public InsertResult(bool success, ResultError errorCode)
            : base(success, errorCode)
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

        public override void SetCompleted(bool success, ResultError errorCode)
        {
            Debug.Assert(!success);
            base.SetCompleted(false, errorCode);
        }
        #endregion

        #region Debugging
        public override string ToString()
        {
            if (Success)
                return $"Success, last id={LastCreatedKeyId}";
            else if (IsCompleted)
                return $"Failed";
            else
                return $"...";
        }
        #endregion
    }
}
