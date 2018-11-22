using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column with values of type <see cref="bool"/>.
    /// </summary>
    public interface IColumnTypeBoolean : IColumnTypeBase<bool>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type <see cref="bool"/>.
    /// </summary>
    public class ColumnTypeBoolean : ColumnTypeBase<bool>, IColumnTypeBoolean
    {
        #region Init
        private ColumnTypeBoolean() { }
        internal static IColumnTypeBoolean Instance = new ColumnTypeBoolean();
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType { get { return "TINYINT"; } }
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
        public override string ToSqlFormat(bool value)
        {
            return value ? "1" : "0";
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to <see cref="bool"/>.
        /// </returns>
        public override bool FromSqlFormat(object sqlValue)
        {
            Debug.Assert(sqlValue is sbyte);
            if (sqlValue is sbyte)
                return ((sbyte)sqlValue) != 0;

            return default;
        }
        #endregion
    }
}
