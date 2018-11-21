using System;
using System.Collections.Generic;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents initial parameters of a request to delete rows in a table, with constraints.
    /// </summary>
    public interface IDeleteContext : IMultiRowConstrainableContext, IModifyContext
    {
        /// <summary>
        ///     Gets the expected minimum number of rows to be deleted.
        /// </summary>
        /// <returns>
        ///     The expected minimum number of rows to be deleted.
        /// </returns>
        int ExpectedDeletedCount { get; }
    }
    #endregion

    /// <summary>
    ///     Represents initial parameters of a request to delete rows in a table, with constraints.
    /// </summary>
    public class DeleteContext : MultiRowConstrainableContext, IDeleteContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteContext"/> class.
        ///     This request has no constraints.
        ///     This request will be considered successful only if it deletes at least <paramref name="expectedDeletedCount"/> rows.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="expectedDeletedCount">The expected minimum number of rows to be deleted.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="expectedDeletedCount"/> is negative.
        /// </exception>
        public DeleteContext(ITableDescriptor table, int expectedDeletedCount)
            : base(table)
        {
            if (expectedDeletedCount < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedDeletedCount));
            ExpectedDeletedCount = expectedDeletedCount;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteContext"/> class.
        ///     This request has a constraint: one column must match one value.
        ///     This request will be considered successful only if it deletes at least <paramref name="expectedDeletedCount"/> rows.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraint">The column and value it must take.</param>
        /// <param name="expectedDeletedCount">The expected minimum number of rows to be deleted.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="expectedDeletedCount"/> is negative.
        /// </exception>
        public DeleteContext(ITableDescriptor table, IColumnValuePair constraint, int expectedDeletedCount)
            : base(table, constraint)
        {
            if (expectedDeletedCount < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedDeletedCount));
            ExpectedDeletedCount = expectedDeletedCount;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteContext"/> class.
        ///     This request has a constraint: one column must match one value among many.
        ///     This request will be considered successful only if it deletes at least <paramref name="expectedDeletedCount"/> rows.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraint">The column and values it must take to match.</param>
        /// <param name="expectedDeletedCount">The expected minimum number of rows to be deleted.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="expectedDeletedCount"/> is negative.
        /// </exception>
        public DeleteContext(ITableDescriptor table, IColumnValueCollectionPair constraint, int expectedDeletedCount)
            : base(table, constraint)
        {
            if (expectedDeletedCount < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedDeletedCount));
            ExpectedDeletedCount = expectedDeletedCount;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteContext"/> class.
        ///     This request has a constraint: several columns must match a single value.
        ///     This request will be considered successful only if it deletes at least <paramref name="expectedDeletedCount"/> rows.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraintList">The columns and value they must take to match.</param>
        /// <param name="expectedDeletedCount">The expected minimum number of rows to be deleted.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="expectedDeletedCount"/> is negative.
        /// </exception>
        public DeleteContext(ITableDescriptor table, IEnumerable<IColumnValuePair> constraintList, int expectedDeletedCount)
            : base(table, constraintList)
        {
            if (expectedDeletedCount < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedDeletedCount));
            ExpectedDeletedCount = expectedDeletedCount;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the expected minimum number of rows to be deleted.
        /// </summary>
        /// <returns>
        ///     The expected minimum number of rows to be deleted.
        /// </returns>
        public int ExpectedDeletedCount { get; }
        #endregion
    }
}
