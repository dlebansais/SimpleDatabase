using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request to update values in a row of a table, with constraints on the previous values.
    /// </summary>
    public interface IUpdateContext : ISingleRowConstrainableContext, IModifyContext, IDataContext
    {
        /// <summary>
        ///     Gets the colums and new values to update.
        /// </summary>
        /// <returns>
        ///     The colums and new values to update.
        /// </returns>
        IReadOnlyCollection<IColumnValuePair> EntryList { get; }
    }

    /// <summary>
    ///     Represents initial parameters of a request to update values in a row of a table, with constraints on the previous values.
    /// </summary>
    public class UpdateContext : SingleRowConstrainableContext, IUpdateContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateContext"/> class.
        ///     This request has a constraint: one column must match one value.
        ///     If <paramref name="entryList"/> is empty, nothing is updated and the request returns success immediately.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraint">The column and value it must take.</param>
        /// <param name="entryList">The colums and new values to update.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="entryList"/> is null.
        /// </exception>
        public UpdateContext(ITableDescriptor table, IColumnValuePair constraint, IEnumerable<IColumnValuePair> entryList)
            : base(table, constraint)
        {
            InitEntryList(entryList);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateContext"/> class.
        ///     This request has a constraint: several columns must match a given value.
        ///     If <paramref name="constraintList"/> is empty, no constraint will be applied to the request.
        ///     If <paramref name="entryList"/> is empty, nothing is updated and the request returns success immediately.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraintList">The columns and value they must take to match.</param>
        /// <param name="entryList">The colums and new values to update.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="entryList"/> is null.
        /// </exception>
        public UpdateContext(ITableDescriptor table, IEnumerable<IColumnValuePair> constraintList, IEnumerable<IColumnValuePair> entryList)
            : base(table, constraintList)
        {
            InitEntryList(entryList);
        }

        private void InitEntryList(IEnumerable<IColumnValuePair> entryList)
        {
            if (entryList == null)
                throw new ArgumentNullException(nameof(entryList));

            EntryList = new List<IColumnValuePair>(entryList);
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the colums and new values to update.
        /// </summary>
        /// <returns>
        ///     The colums and new values to update.
        /// </returns>
        public IReadOnlyCollection<IColumnValuePair> EntryList { get; private set; }
        #endregion
    }
}
