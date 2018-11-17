using System;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column containing int values.
    /// </summary>
    public interface IColumnDescriptorInt : IColumnDescriptorBase<int>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column containing int values.
    /// </summary>
    public class ColumnDescriptorInt : ColumnDescriptorBase<int>, IColumnDescriptorInt
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescriptorInt"/> class and adds it to the table it belongs to.
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
        public ColumnDescriptorInt(ITableDescriptor table, string name)
            : base(table, name, new ColumnTypeInt())
        {
        }
        #endregion
    }
}
