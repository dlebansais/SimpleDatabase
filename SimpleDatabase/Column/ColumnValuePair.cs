using Database.Types;
using System;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents a column and its value.
    /// </summary>
    public interface IColumnValuePair
    {
        /// <summary>
        ///     Gets the column.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnDescriptor"/> describing the column.
        /// </returns>
        IColumnDescriptor Column { get; }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <returns>
        ///     An <see cref="object"/>.
        /// </returns>
        object Value { get; }
    }

    /// <summary>
    ///     Represents a column and its value of type <typeparamref name="T"/>.
    /// </summary>
    public interface IColumnValuePair<T>
    {
        /// <summary>
        ///     Gets the column.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnDescriptor{T}"/> describing the column.
        /// </returns>
        IColumnDescriptor<T> Column { get; }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <returns>
        ///     An object of type <typeparamref name="T"/>.
        /// </returns>
        T Value { get; }
    }
    #endregion

    /// <summary>
    ///     Represents a column and its value of type <typeparamref name="T"/>.
    /// </summary>
    public class ColumnValuePair<T> : IColumnValuePair<T>, IColumnValuePair
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="IColumnDescriptor{T}"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="column">The column that contains the value.</param>
        /// <param name="value">The value for the column.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="column"/> is null.
        /// </exception>
        public ColumnValuePair(IColumnDescriptor<T> column, T value)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the column.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnDescriptor{T}"/> describing the column.
        /// </returns>
        public IColumnDescriptor<T> Column { get; }
        IColumnDescriptor IColumnValuePair.Column { get { return Column; } }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <returns>
        ///     An object of type <typeparamref name="T"/>.
        /// </returns>
        public T Value { get; }
        object IColumnValuePair.Value { get { return Value; } }
        #endregion

        #region Debugging
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return $"{Column.Name}={Value}";
        }
        #endregion
    }
}
