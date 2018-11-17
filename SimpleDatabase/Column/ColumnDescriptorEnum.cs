using System;

namespace Database.Types
{
    #region Interface
    /// <summary>
    /// Represents a column containing enumeration values.
    /// </summary>
    public interface IColumnDescriptorEnum<TEnum> : IColumnDescriptorBase<Enum>
         where TEnum : struct, Enum
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column containing enumeration values.
    /// </summary>
    public abstract class ColumnDescriptorEnum<TEnum> : ColumnDescriptorBase<Enum>, IColumnDescriptorEnum<TEnum>
         where TEnum : struct, Enum
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescriptorEnum{TEnum}"/> class and adds it to the table it belongs to.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table containing the column.</param>
        /// <param name="name">The column name.</param>
        /// <param name="type">The corresponding column type.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="type"/> is null.
        /// </exception>
        public ColumnDescriptorEnum(ITableDescriptor table, string name, IColumnTypeEnum<TEnum> type)
            : base(table, name, type)
        {
        }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Try to parse a row containing a value for this column.
        /// </summary>
        /// <parameters>
        /// <param name="row">The row to parse.</param>
        /// <param name="value">The value, if parsed successfully.</param>
        /// </parameters>
        /// <returns>
        ///     True if parsed successfully, False otherwise and in that case <paramref name="value"/> is unspecified.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="row"/> is null.
        /// </exception>
        public bool TryParseRow(IResultRow row, out TEnum value)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (TryParseRow(row, out Enum EnumValue))
            {
                value = (TEnum)(object)EnumValue;
                return true;
            }
            else
            {
                value = default(TEnum);
                return false;
            }
        }
        #endregion
    }
}
