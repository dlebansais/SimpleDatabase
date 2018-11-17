using System;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request to insert values in a table.
    /// </summary>
    public interface IInsertContext : ISingleTableOperationContext, IModifyContext
    {
    }

    /// <summary>
    ///     Represents initial parameters of a request to insert values in a table.
    /// </summary>
    public abstract class InsertContext : SingleTableOperationContext, IInsertContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="InsertContext"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        public InsertContext(ITableDescriptor table)
            : base(table)
        {
        }
        #endregion
    }
}
