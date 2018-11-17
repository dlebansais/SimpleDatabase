using System;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents the type of a column for values of type <typeparamref name="T"/>.
    /// </summary>
    public interface IColumnTypeBase<T> : IColumnType<T>
    {
    }
    #endregion

    /// <summary>
    ///     Represents the type of a column for values of type <typeparamref name="T"/>.
    /// </summary>
    public abstract class ColumnTypeBase<T> : IColumnTypeBase<T>
    {
        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public abstract string SqlType { get; }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Gets the string that corresponds to <paramref name="value"/> to use in a SQL request.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value.</param>
        /// </parameters>
        /// <returns>
        ///     A string that represents <paramref name="value"/> for a SQL request.
        /// </returns>
        public abstract string ToSqlFormat(T value);

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
        public virtual string ToSqlFormat(object value)
        {
            if (value is T)
                return ToSqlFormat((T)value);
            else
                throw new ArgumentException(nameof(value));
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to type <typeparamref name="T"/>.
        /// </returns>
        public abstract T FromSqlFormat(object sqlValue);
        #endregion
    }
}
