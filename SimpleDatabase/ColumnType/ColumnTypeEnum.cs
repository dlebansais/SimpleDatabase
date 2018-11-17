using System;
using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column with values of type <typeparamref name="TEnum"/>, an enumeration type.
    /// </summary>
    public interface IColumnTypeEnum<TEnum> : IColumnTypeBase<Enum>
         where TEnum : struct, Enum
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column with values of type <typeparamref name="TEnum"/>, an enumeration type.
    /// </summary>
    public abstract class ColumnTypeEnum<TEnum> : ColumnTypeBase<Enum>, IColumnTypeEnum<TEnum>
         where TEnum : struct, Enum
    {
        #region Properties
        /// <summary>
        ///     Gets the underlying SQL type.
        /// </summary>
        /// <returns>
        ///     The underlying SQL type.
        /// </returns>
        public override string SqlType
        {
            get
            {
                string[] EnumNames = typeof(TEnum).GetEnumNames();
                string EnumList = "";
                foreach (string EnumName in EnumNames)
                {
                    if (EnumList.Length > 0)
                        EnumList += ", ";

                    EnumList += $"'{EnumName}'";
                }

                return $"ENUM({EnumList})";
            }
        }
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
        public override string ToSqlFormat(Enum value)
        {
            return $"'{value}'";
        }

        /// <summary>
        ///     Parses an object obtained from a SQL request and return the corresponding value.
        /// </summary>
        /// <parameters>
        /// <param name="sqlValue">The object obtained from a SQL request.</param>
        /// </parameters>
        /// <returns>
        ///     <paramref name="sqlValue"/> converted to <typeparamref name="TEnum"/>.
        /// </returns>
        public override Enum FromSqlFormat(object sqlValue)
        {
            Debug.Assert(sqlValue is string);
            if (sqlValue is string)
                if (Enum.TryParse(sqlValue as string, out TEnum Result))
                    return Result;

            return default(TEnum);
        }
        #endregion
    }
}
