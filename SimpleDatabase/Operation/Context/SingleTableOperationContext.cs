using System;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents initial parameters of a request over only one table.
    /// </summary>
    public interface ISingleTableOperationContext : IContext
    {
        /// <summary>
        ///     Gets the table the request is addressing.
        /// </summary>
        /// <returns>
        ///     The table the request is addressing.
        /// </returns>
        ITableDescriptor Table { get; }
    }
    #endregion

    /// <summary>
    ///     Represents initial parameters of a request over only one table.
    /// </summary>
    public class SingleTableOperationContext : Context, ISingleTableOperationContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleTableOperationContext"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        public SingleTableOperationContext(ITableDescriptor table)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the table the request is addressing.
        /// </summary>
        /// <returns>
        ///     The table the request is addressing.
        /// </returns>
        public ITableDescriptor Table { get; }
        #endregion
    }
}
