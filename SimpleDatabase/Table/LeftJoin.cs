using Database.Types;
using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents a LEFT JOIN table specification.
    /// </summary>
    public interface ILeftJoin : IJoin
    {
    }

    /// <summary>
    ///     Represents a LEFT JOIN table specification.
    /// </summary>
    public class LeftJoin : Join, ILeftJoin
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LeftJoin"/> class.
        ///     This constructor creates a "table1 LEFT JOIN table2 ON <paramref name="column1"/>=<paramref name="column2"/>" specification, where table1 is the table containing <paramref name="column1"/> and table2 the table containing <paramref name="column2"/>.
        /// </summary>
        /// <parameters>
        /// <param name="column1">The column participating to the join, in the table on the left of LEFT JOIN.</param>
        /// <param name="column2">The column participating to the join, in the table on the right of LEFT JOIN.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="column1"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="column2"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="column1"/> and <paramref name="column2"/> belong to the same table.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="column1"/> and <paramref name="column2"/> are not of the same type.
        /// </exception>
        public LeftJoin(IColumnDescriptor column1, IColumnDescriptor column2)
        {
            if (column1 == null)
                throw new ArgumentNullException(nameof(column1));
            if (column2 == null)
                throw new ArgumentNullException(nameof(column1));
            if (column1.Table == column2.Table)
                throw new ArgumentException("Invalid join");
            if (column1.Type != column2.Type)
                throw new ArgumentException("Invalid join");

            Columns = new Dictionary<IColumnDescriptor, IColumnDescriptor>()
            {
                { column1, column2 }
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LeftJoin"/> class.
        ///     This constructor creates a "table1 LEFT JOIN table2 ON column1=column2 LEFT JOIN table3 ON..." specification.
        /// </summary>
        /// <parameters>
        /// <param name="columns">The list of columns participating to the join.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="columns"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="columns"/> is empty, or specifies an invalid join.
        /// </exception>
        public LeftJoin(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> columns)
        {
            if (columns == null)
                throw new ArgumentNullException(nameof(columns));
            if (columns.Count == 0 || !IsJoinValid(columns))
                throw new ArgumentException("Invalid join");
            
            Columns = columns;
        }

        private bool IsJoinValid(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> columns)
        {
            Dictionary<IColumnDescriptor, IColumnDescriptor> TestJoin = new Dictionary<IColumnDescriptor, IColumnDescriptor>();
            foreach (KeyValuePair<IColumnDescriptor, IColumnDescriptor> Entry in columns)
                TestJoin.Add(Entry.Key, Entry.Value);

            while (TestJoin.Count > 0)
            {
                IColumnDescriptor TestColumn = null;
                foreach (KeyValuePair<IColumnDescriptor, IColumnDescriptor> Entry in TestJoin)
                {
                    TestColumn = Entry.Key;
                    break;
                }

                IColumnDescriptor NextColumn = TestColumn;
                while (NextColumn != null && TestJoin.ContainsKey(NextColumn))
                {
                    NextColumn = TestJoin[NextColumn];
                    TestJoin.Remove(TestColumn);

                    if (NextColumn == TestColumn)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Gets the joined tables by their column equality specifier.
        /// </summary>
        /// <returns>
        ///     The joined tables by their column equality specifier.
        /// </returns>
        public override IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> Columns { get; }
    }
}
