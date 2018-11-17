using System;
using System.Collections;
using System.Collections.Generic;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column, in a table of a schema, containing any type of value.
    /// </summary>
    public interface IColumnDescriptor
    {
        /// <summary>
        ///     Gets the table.
        /// </summary>
        /// <returns>
        ///     The table to which the column belongs.
        /// </returns>
        ITableDescriptor Table { get; }

        /// <summary>
        ///     Gets the column name.
        /// </summary>
        /// <returns>
        ///     The column name.
        /// </returns>
        string Name { get; }

        /// <summary>
        ///     Gets the column type.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnType"/> object that represents the type of values the column can hold.
        /// </returns>
        IColumnType Type { get; }

        /// <summary>
        ///     Associates a value with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValuePair"/> representing the column and value pair.
        ///     Returns null if <paramref name="value"/> is of the wrong type.
        /// </returns>
        IColumnValuePair CreateValuePair(object value);

        /// <summary>
        ///     Associates a collection of values with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="collection">The collection of values to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValueCollectionPair"/> representing the column and collection of values pair.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="collection"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="collection"/> is a collection of items of the wrong type.
        /// </exception>
        IColumnValueCollectionPair CreateValueCollectionPair(IEnumerable collection);
    }

    /// <summary>
    ///     Represents a column, in a table of a schema, containing values of type <typeparamref name="T"/>.
    /// </summary>
    public interface IColumnDescriptor<T> : IColumnDescriptor
    {
        /// <summary>
        ///     Associates a value with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value of type <typeparamref name="T"/> to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValuePair{T}"/> representing the column and value pair.
        /// </returns>
        IColumnValuePair<T> CreateValuePair(T value);

        /// <summary>
        ///     Associates a collection of values with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="collection">The collection of values of type <typeparamref name="T"/> to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValueCollectionPair{T}"/> representing the column and collection of values pair.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="collection"/> is null.
        /// </exception>
        IColumnValueCollectionPair<T> CreateValueCollectionPair(IEnumerable<T> collection);

        /// <summary>
        ///     Try to parse a row containing a value for this column.
        /// </summary>
        /// <parameters>
        /// <param name="row">The row to parse.</param>
        /// <param name="value">The value of type <typeparamref name="T"/>, if parsed successfully.</param>
        /// </parameters>
        /// <returns>
        ///     True if parsed successfully, False otherwise and in that case <paramref name="value"/> is unspecified.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="row"/> is null.
        /// </exception>
        bool TryParseRow(IResultRow row, out T value);
    }
    #endregion
}
