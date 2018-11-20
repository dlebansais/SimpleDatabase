using Database.Types;
using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request over several tables.
    /// </summary>
    public interface IJoinOperationContext : IContext
    {
        /// <summary>
        ///     Gets the join describing how tables are connected in the query.
        /// </summary>
        /// <returns>
        ///     The join describing how tables are connected in the query.
        /// </returns>
        IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> Join { get; }
    }

    /// <summary>
    ///     Represents initial parameters of a request over several tables.
    /// </summary>
    public class JoinOperationContext : Context, IJoinOperationContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinOperationContext"/> class.
        ///     This instance has no join and addresses only one table.
        /// </summary>
        public JoinOperationContext()
        {
            Join = new Dictionary<IColumnDescriptor, IColumnDescriptor>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinOperationContext"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="join">The join describing how tables are connected in the request.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="join"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="join"/> does not describe a valid join.
        /// </exception>
        public JoinOperationContext(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> join)
        {
            Join = join ?? throw new ArgumentNullException(nameof(join));
            if (!IsJoinValid(join))
                throw new ArgumentException(nameof(join));
        }

        private bool IsJoinValid(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> join)
        {
            Dictionary<IColumnDescriptor, IColumnDescriptor> TestJoin = new Dictionary<IColumnDescriptor, IColumnDescriptor>();
            foreach (KeyValuePair<IColumnDescriptor, IColumnDescriptor> Entry in Join)
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
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the join describing how tables are connected in the query.
        /// </summary>
        /// <returns>
        ///     The join describing how tables are connected in the query.
        /// </returns>
        public IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> Join { get; }
        #endregion
    }
}
