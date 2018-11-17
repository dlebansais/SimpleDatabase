using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request to insert a single row of values in a table.
    /// </summary>
    public interface ISingleInsertContext : IInsertContext, IDataContext
    {
        /// <summary>
        ///     Gets the columns and their value to insert.
        /// </summary>
        /// <returns>
        ///     The columns and their value to insert.
        /// </returns>
        IReadOnlyCollection<IColumnValuePair> EntryList { get; }
    }

    /// <summary>
    ///     Represents initial parameters of a request to insert a single row of values in a table.
    /// </summary>
    public class SingleInsertContext : InsertContext, ISingleInsertContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleInsertContext"/> class.
        ///     Creating a request with zero columns is valid, and the corresponding operation will always return success with no row inserted immediately.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="entryList">The columns and their value to insert.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="entryList"/> is null.
        /// </exception>
        public SingleInsertContext(ITableDescriptor table, IEnumerable<IColumnValuePair> entryList)
            : base(table)
        {
            if (entryList == null)
                throw new ArgumentNullException(nameof(entryList));
            EntryList = new List<IColumnValuePair>(entryList);
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the columns and their value to insert.
        /// </summary>
        /// <returns>
        ///     The columns and their value to insert.
        /// </returns>
        public IReadOnlyCollection<IColumnValuePair> EntryList { get; }
        #endregion
    }
}
