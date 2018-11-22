using System;
using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column with values of type <see cref="Guid"/>.
    /// </summary>
    public interface IColumnTypeGuid : IColumnTypeBase<Guid>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type <see cref="Guid"/>.
    /// </summary>
    public class ColumnTypeGuid : ColumnTypeBase<Guid>, IColumnTypeGuid
    {
        #region Init
        private ColumnTypeGuid() { }
        internal static IColumnTypeGuid Instance = new ColumnTypeGuid();
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType { get { return "CHAR(38)"; } }
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
        public override string ToSqlFormat(Guid value)
        {
            return $"'{value.ToString("B")}'";
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to <see cref="Guid"/>.
        /// </returns>
        public override Guid FromSqlFormat(object sqlValue)
        {
            Debug.Assert(sqlValue is string);
            if (sqlValue is string)
                return Guid.ParseExact(sqlValue as string, "B");

            return default;
        }
        #endregion
    }
}
