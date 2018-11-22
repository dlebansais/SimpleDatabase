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
            : base(table, name, ColumnTypeGuid.Instance)
        {
        }
        #endregion

        #region Debugging
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return $"{Name} (Guid)";
        }
        #endregion
    }
}
