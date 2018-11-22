using Database.Types;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents a join specification that addresses only one table.
    /// </summary>
    public class EmptyJoin : Join
    {
        /// <summary>
        ///     Represents a join specification that addresses only one table.
        /// </summary>
        public static IJoin Join = new EmptyJoin();

        private EmptyJoin()
        {
            Columns = new Dictionary<IColumnDescriptor, IColumnDescriptor>();
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
