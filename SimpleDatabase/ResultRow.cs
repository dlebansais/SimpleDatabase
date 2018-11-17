using Database.Types;
using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents values returned by a query.
    /// </summary>
    public interface IResultRow
    {
        /// <summary>
        ///     Inidcates if the row contains a value for the specified column.
        /// </summary>
        /// <parameters>
        /// <param name="column">The column to look for.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="column"/> is null.
        /// </exception>
        /// <returns>
        ///     True if the row contains a value for <paramref name="column"/>, False otherwise.
        /// </returns>
        bool HasColumn(IColumnDescriptor column);
    }
}

namespace Database
{
    internal interface IResultRowInternal : IResultRow
    {
        IDictionary<IColumnDescriptor, object> ColumnTable { get; }
        void AddResult(IColumnDescriptor Column, object Value);
    }

    internal class ResultRow : IResultRowInternal
    {
        #region Init
        public ResultRow()
        {
            ColumnTable = new Dictionary<IColumnDescriptor, object>();
        }
        #endregion

        #region Properties
        public IDictionary<IColumnDescriptor, object> ColumnTable { get; }
        #endregion

        #region Client Interface
        public bool HasColumn(IColumnDescriptor column)
        {
            return ColumnTable.ContainsKey(column);
        }

        public virtual void AddResult(IColumnDescriptor Column, object Value)
        {
            ColumnTable.Add(Column, Value);
        }
        #endregion
    }
}
