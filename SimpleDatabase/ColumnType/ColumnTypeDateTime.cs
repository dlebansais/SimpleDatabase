using System;
using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column with values of type <see cref="DateTime"/>.
    /// </summary>
    public interface IColumnTypeDateTime : IColumnTypeBase<DateTime>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type <see cref="DateTime"/>.
    /// </summary>
    public class ColumnTypeDateTime : ColumnTypeBase<DateTime>, IColumnTypeDateTime
    {
        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType { get { return "BIGINT"; } }
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
        public override string ToSqlFormat(DateTime value)
        {
            return value.Ticks.ToString();
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to <see cref="DateTime"/>.
        /// </returns>
        public override DateTime FromSqlFormat(object sqlValue)
        {
            Debug.Assert(sqlValue is long);
            if (sqlValue is long)
                return new DateTime((long)sqlValue);

            return default;
        }
        #endregion
    }
}
