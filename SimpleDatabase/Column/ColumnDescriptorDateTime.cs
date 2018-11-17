﻿using System;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column containing DateTime values.
    /// </summary>
    public interface IColumnDescriptorDateTime : IColumnDescriptorBase<DateTime>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column containing DateTime values.
    /// </summary>
    public class ColumnDescriptorDateTime : ColumnDescriptorBase<DateTime>, IColumnDescriptorDateTime
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescriptorDateTime"/> class and adds it to the table it belongs to.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table containing the column.</param>
        /// <param name="name">The column name.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null or empty.
        /// </exception>
        public ColumnDescriptorDateTime(ITableDescriptor table, string name)
            : base(table, name, new ColumnTypeDateTime())
        {
        }
        #endregion
    }
}