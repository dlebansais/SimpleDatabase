using System;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents the type of a column.
    /// </summary>
    public interface IColumnType
    {
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        string SqlType { get; }

        /// <summary>
        ///     Gets the string that corresponds to <paramref name="value"/> to use in a SQL request.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value.</param>
        /// </parameters>
        /// <returns>
        ///     A string that represents <paramref name="value"/> for a SQL request.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="value"/> is of the wrong type.
        /// </exception>
        string ToSqlFormat(object value);
    }

    /// <summary>
    ///     Represents the type of a column.
    /// </summary>
    public interface IColumnType<T> : IColumnType
    {
        /// <summary>
        ///     Gets the string that corresponds to <paramref name="value"/> to use in a SQL request.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value.</param>
        /// </parameters>
        /// <returns>
        ///     A string that represents <paramref name="value"/> for a SQL request.
        /// </returns>
        string ToSqlFormat(T value);

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to type <typeparamref name="T"/>.
        /// </returns>
        T FromSqlFormat(object sqlValue);
    }
    #endregion
}
