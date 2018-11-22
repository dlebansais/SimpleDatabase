namespace Database
{
    /// <summary>
    ///     Error codes.
    /// </summary>
    public enum ResultError
    {
        /// <summary>
        ///     No error.
        /// </summary>
        ErrorNone = 0,

        /// <summary>
        ///     Error reported when not enough rows have been deleted.
        /// </summary>
        ErrorMissingDeletedRows = 1,

        /// <summary>
        ///     Error reported when an exception occured while completing the operation.
        /// </summary>
        ErrorExceptionCompletingDelete = 2,

        /// <summary>
        ///     Error reported when an insert operation failed to insert rows.
        /// </summary>
        ErrorLineNotInserted = 3,

        /// <summary>
        ///     Error reported when an exception occured while completing the operation.
        /// </summary>
        ErrorExceptionCompletingInsert = 4,

        /// <summary>
        ///     Error reported when an exception occured while completing the operation.
        /// </summary>
        ErrorExceptionCompletingJoinQuery = 5,

        /// <summary>
        ///     Error reported when an exception occured while completing the operation.
        /// </summary>
        ErrorExceptionCompletingQuery = 6,

        /// <summary>
        ///     Error reported when an update operation did not change any row.
        /// </summary>
        ErrorNoRowModified = 7,

        /// <summary>
        ///     Error reported when an exception occured while completing the operation.
        /// </summary>
        ErrorExceptionCompletingUpdate = 8,

        /// <summary>
        ///     Error reported when a row is inserted without a required primary key.
        /// </summary>
        ErrorPrimaryKeyRequired = 9,

        /// <summary>
        ///     Error reported when the operation thread failed to start.
        /// </summary>
        ErrorFatalNoOperationThread = 10,
    }
}
