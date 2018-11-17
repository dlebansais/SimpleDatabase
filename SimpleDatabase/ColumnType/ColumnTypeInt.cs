using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column with values of type <see cref="int"/>.
    /// </summary>
    public interface IColumnTypeInt : IColumnTypeBase<int>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type <see cref="int"/>.
    /// </summary>
    public class ColumnTypeInt : ColumnTypeBase<int>, IColumnTypeInt
    {
        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType { get { return "INT"; } }
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
        public override string ToSqlFormat(int value)
        {
            return value.ToString();
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to <see cref="int"/>.
        /// </returns>
        public override int FromSqlFormat(object sqlValue)
        {
            Debug.Assert(sqlValue is int);
            if (sqlValue is int)
                return (int)sqlValue;

            return default;
        }
        #endregion
    }
}
