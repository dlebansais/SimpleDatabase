using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column with values of type <see cref="string"/>.
    /// </summary>
    public interface IColumnTypeString : IColumnTypeBase<string>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type <see cref="string"/>.
    /// </summary>
    public class ColumnTypeString : ColumnTypeBase<string>, IColumnTypeString
    {
        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType { get { return "TEXT"; } }
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
        public override string ToSqlFormat(string value)
        {
            return "'" + value + "'";
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to <see cref="string"/>.
        /// </returns>
        public override string FromSqlFormat(object sqlValue)
        {
            Debug.Assert(sqlValue is string);
            if (sqlValue is string)
                return sqlValue as string;

            return default;
        }
        #endregion
    }
}
