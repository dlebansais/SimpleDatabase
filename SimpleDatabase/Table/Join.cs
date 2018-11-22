using Database.Types;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents a join specification.
    /// </summary>
    public interface IJoin
    {
        /// <summary>
        ///     Gets the joined tables by their column equality specifier.
        /// </summary>
        /// <returns>
        ///     The joined tables by their column equality specifier.
        /// </returns>
        IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> Columns { get; }
    }

    /// <summary>
    ///     Represents a join specification.
    /// </summary>
    public abstract class Join : IJoin
    {
        /// <summary>
        ///     Gets the joined tables by their column equality specifier.
        /// </summary>
        /// <returns>
        ///     The joined tables by their column equality specifier.
        /// </returns>
        public abstract IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> Columns { get; }
    }
}
