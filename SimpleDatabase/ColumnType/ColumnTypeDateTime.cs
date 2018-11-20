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
        /// <summary>
        ///     Indicates if the DateTime type is stored as a tick count.
        /// </summary>
        /// <returns>
        ///     True if the DateTime type is stored as a tick count.
        /// </returns>
        bool DateTimeAsTicks { get; }
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type <see cref="DateTime"/>.
    /// </summary>
    public class ColumnTypeDateTime : ColumnTypeBase<DateTime>, IColumnTypeDateTime
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnTypeDateTime"/> class and adds it to the table it belongs to.
        /// </summary>
        /// <parameters>
        /// <param name="dateTimeAsTicks">Indicates if the DateTime type is stored as a tick count.</param>
        /// </parameters>
        public ColumnTypeDateTime(bool dateTimeAsTicks)
        {
            DateTimeAsTicks = dateTimeAsTicks;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Indicates if the DateTime type is stored as a tick count.
        /// </summary>
        /// <returns>
        ///     True if the DateTime type is stored as a tick count.
        /// </returns>
        public bool DateTimeAsTicks { get; }

        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType { get { return DateTimeAsTicks ? "BIGINT" : "DATETIME"; } }
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
            if (DateTimeAsTicks)
                return value.Ticks.ToString();
            else
                return value.ToString();
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
            if (DateTimeAsTicks)
            {
                Debug.Assert(sqlValue is long);
                if (sqlValue is long)
                    return new DateTime((long)sqlValue);
            }
            else
            {
                Debug.Assert(sqlValue is DateTime);
                if (sqlValue is DateTime)
                    return (DateTime)sqlValue;
            }

            return default;
        }
        #endregion
    }
}
