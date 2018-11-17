using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column with values of type byte[].
    /// </summary>
    public interface IColumnTypeByteArray : IColumnTypeBase<byte[]>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type byte[].
    /// </summary>
    public class ColumnTypeByteArray : ColumnTypeBase<byte[]>, IColumnTypeByteArray
    {
        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType { get { return "LONGBLOB"; } }
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
        public override string ToSqlFormat(byte[] value)
        {
            return "''";
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to bool.
        /// </returns>
        public override byte[] FromSqlFormat(object sqlValue)
        {
            Debug.Assert(sqlValue is byte[]);
            if (sqlValue is byte[])
                return (byte[])sqlValue;

            return default;
        }
        #endregion
    }
}
