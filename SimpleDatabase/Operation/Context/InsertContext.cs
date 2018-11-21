using System;
using System.Collections;
using System.Collections.Generic;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents initial parameters of a request to insert several values in a table.
    /// </summary>
    public interface IInsertContext : ISingleTableOperationContext, IModifyContext, IDataContext
    {
        /// <summary>
        ///     Gets the number of values to insert.
        /// </summary>
        /// <returns>
        ///     The number of values to insert.
        /// </returns>
        int RowCount { get; }

        /// <summary>
        ///     Gets the columns with values to insert.
        /// </summary>
        /// <returns>
        ///     The columns with values to insert.
        /// </returns>
        IReadOnlyCollection<IColumnValueCollectionPair> EntryList { get; }
    }
    #endregion

    /// <summary>
    ///     Represents initial parameters of a request to insert several values in a table.
    /// </summary>
    public class InsertContext : SingleTableOperationContext, IInsertContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="InsertContext"/> class.
        ///     This instance will insert a single row.
        ///     Creating a request with zero columns is valid, and the corresponding operation will always return success immediately.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="entryList">The columns with a value to insert.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="entryList"/> is null.
        /// </exception>
        public InsertContext(ITableDescriptor table, IEnumerable<IColumnValuePair> entryList)
            : base(table)
        {
            if (entryList == null)
                throw new ArgumentNullException(nameof(entryList));

            List<IColumnValueCollectionPair> InitEntryList = new List<IColumnValueCollectionPair>();
            foreach (IColumnValuePair Entry in entryList)
                InitEntryList.Add(Entry.GetAsCollection());

            RowCount = 1;
            EntryList = InitEntryList.AsReadOnly();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InsertContext"/> class.
        ///     Creating a request with zero rows or columns is valid, and the corresponding operation will always return success immediately.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="rowCount">The number of values to insert.</param>
        /// <param name="entryList">The columns with values to insert.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="rowCount"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="entryList"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     One of the columns in <paramref name="entryList"/> does not have exactly <paramref name="rowCount"/> values to insert.
        /// </exception>
        public InsertContext(ITableDescriptor table, int rowCount, IEnumerable<IColumnValueCollectionPair> entryList)
            : base(table)
        {
            if (rowCount < 0)
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            RowCount = rowCount;

            if (entryList == null)
                throw new ArgumentNullException(nameof(entryList));
            EntryList = new List<IColumnValueCollectionPair>(entryList).AsReadOnly();

            foreach (IColumnValueCollectionPair Entry in EntryList)
            {
                int ValueCount = 0;
                IEnumerator Enumerator = Entry.ValueCollection.GetEnumerator();
                while (Enumerator.MoveNext())
                    ValueCount++;

                if (ValueCount != RowCount)
                    throw new ArgumentException(nameof(entryList));
            }
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the number of values to insert.
        /// </summary>
        /// <returns>
        ///     The number of values to insert.
        /// </returns>
        public int RowCount { get; }

        /// <summary>
        ///     Gets the columns with values to insert.
        /// </summary>
        /// <returns>
        ///     The columns with values to insert.
        /// </returns>
        public IReadOnlyCollection<IColumnValueCollectionPair> EntryList { get; }
        #endregion
    }
}
