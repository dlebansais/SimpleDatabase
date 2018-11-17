using System;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column containing bool values.
    /// </summary>
    public interface IColumnDescriptorBoolean : IColumnDescriptorBase<bool>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column containing bool values.
    /// </summary>
    public class ColumnDescriptorBoolean : ColumnDescriptorBase<bool>, IColumnDescriptorBoolean
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescriptorBoolean"/> class and adds it to the table it belongs to.
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
        public ColumnDescriptorBoolean(ITableDescriptor table, string name)
            : base(table, name, new ColumnTypeBoolean())
        {
        }
        #endregion
    }
}
