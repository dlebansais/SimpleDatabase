using System;
using System.Threading;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents the result of a request.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        ///     Indicates if the request has been started.
        /// </summary>
        /// <returns>
        ///     True if the request has been started, False otherwise.
        /// </returns>
        bool IsStarted { get; }

        /// <summary>
        ///     Indicates if the request has been completed.
        /// </summary>
        /// <returns>
        ///     True if the request has been completed, False otherwise.
        /// </returns>
        bool IsCompleted { get; }

        /// <summary>
        ///     Indicates if the request is successful.
        /// </summary>
        /// <returns>
        ///     True if the request is successful, False if it has not started, has not been completed, or has failed.
        /// </returns>
        bool Success { get; }
    }
    #endregion
}

namespace Database.Internal
{
    #region Interface
    internal interface IResultInternal : IResult
    {
        IOperation Operation { get; }
        IAsyncResult AsyncResult { get; }
        void CheckIfCompletedSynchronously(out bool isCompletedSynchronously);
        bool IsCompletedSynchronously { get; }
        void SetCompleted(bool success);
        void WaitCompleted();
    }
    #endregion

    internal abstract class Result : IResultInternal
    {
        #region Init
        public Result(IOperation operation, IAsyncResult asyncResult)
        {
            Operation = operation;
            AsyncResult = asyncResult;
            Success = false;
            CompletedEvent = new ManualResetEvent(false);
        }

        public Result(bool success)
        {
            Operation = null;
            AsyncResult = null;
            Success = success;
            CompletedEvent = null;
        }
        #endregion

        #region Properties
        public IOperation Operation { get; }
        public bool IsStarted { get { return Success || AsyncResult != null; } }
        public bool IsCompletedSynchronously { get; private set; }
        public bool IsCompleted { get { return Success || CompletedEvent.WaitOne(0); } }
        public bool Success { get; private set; }
        public IAsyncResult AsyncResult { get; }
        #endregion

        #region Client Interface
        public virtual void CheckIfCompletedSynchronously(out bool isCompletedSynchronously)
        {
            IsCompletedSynchronously = AsyncResult != null && AsyncResult.IsCompleted;
            isCompletedSynchronously = IsCompletedSynchronously;
        }

        public virtual void SetCompleted(bool success)
        {
            Success = success;
            CompletedEvent.Set();
        }

        public void WaitCompleted()
        {
            CompletedEvent.WaitOne();
        }

        private ManualResetEvent CompletedEvent;
        #endregion

        #region Debugging
        public override string ToString()
        {
            if (Success)
                return $"Success";
            else if (IsCompleted)
                return $"Failed";
            else
                return $"...";
        }
        #endregion
    }
}
