using Database.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents a column and its collection of values of any type.
    /// </summary>
    public interface IColumnValueCollectionPair
    {
        /// <summary>
        ///     Gets the column.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnDescriptor"/> describing the column.
        /// </returns>
        IColumnDescriptor Column { get; }

        /// <summary>
        ///     Gets the collection of values.
        /// </summary>
        /// <returns>
        ///     A collection of <see cref="object"/>.
        /// </returns>
        IEnumerable ValueCollection { get; }
    }

    /// <summary>
    ///     Represents a column and its collection of values of type <typeparamref name="T"/>.
    /// </summary>
    public interface IColumnValueCollectionPair<T>
    {
        /// <summary>
        ///     Gets the column.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnDescriptor{T}"/> describing the column.
        /// </returns>
        IColumnDescriptor<T> Column { get; }

        /// <summary>
        ///     Gets the collection of values.
        /// </summary>
        /// <returns>
        ///     A collection of objects of type <typeparamref name="T"/>.
        /// </returns>
        IEnumerable<T> ValueCollection { get; }
    }
    #endregion

    /// <summary>
    ///     Represents a column and its collection of values of type <typeparamref name="T"/>.
    /// </summary>
    public class ColumnValueCollectionPair<T> : IColumnValueCollectionPair<T>, IColumnValueCollectionPair
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnValueCollectionPair{T}"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="column">The column that contains values.</param>
        /// <param name="values">The collection of values for the column.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="column"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="values"/> is null.
        /// </exception>
        public ColumnValueCollectionPair(IColumnDescriptor<T> column, IEnumerable<T> values)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            ValueCollection = values ?? throw new ArgumentNullException(nameof(values));
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
        IColumnDescriptor IColumnValueCollectionPair.Column { get { return Column; } }

        /// <summary>
        ///     Gets the collection of values.
        /// </summary>
        /// <returns>
        ///     A collection of objects of type <typeparamref name="T"/>.
        /// </returns>
        public IEnumerable<T> ValueCollection { get; }
        IEnumerable IColumnValueCollectionPair.ValueCollection { get { return ValueCollection; } }
        #endregion

        #region Debugging
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (ValueCollection is ICollection AsCollection)
                return $"{Column.Name}, {AsCollection.Count} element(s)";
            else
                return $"{Column.Name} ...";
        }
        #endregion
    }
}
