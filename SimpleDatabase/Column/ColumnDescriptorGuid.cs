using System;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column containing Guid values.
    /// </summary>
    public interface IColumnDescriptorGuid : IColumnDescriptorBase<Guid>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column containing Guid values.
    /// </summary>
    public class ColumnDescriptorGuid : ColumnDescriptorBase<Guid>, IColumnDescriptorGuid
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescriptorGuid"/> class and adds it to the table it belongs to.
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
        public ColumnDescriptorGuid(ITableDescriptor table, string name)
            : base(table, name, new ColumnTypeGuid())
        {
        }
        #endregion
    }
}
