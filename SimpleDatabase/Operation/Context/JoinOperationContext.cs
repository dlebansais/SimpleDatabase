using System;

namespace Database
{
    #region Interface
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
        IJoin Join { get; }
    }
    #endregion

    /// <summary>
    ///     Represents initial parameters of a request over several tables.
    /// </summary>
    public abstract class JoinOperationContext : Context, IJoinOperationContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinOperationContext"/> class.
        ///     This instance has no join and addresses only one table.
        /// </summary>
        public JoinOperationContext()
        {
            Join = EmptyJoin.Join;
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
        public JoinOperationContext(IJoin join)
        {
            Join = join ?? throw new ArgumentNullException(nameof(join));
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the join describing how tables are connected in the query.
        /// </summary>
        /// <returns>
        ///     The join describing how tables are connected in the query.
        /// </returns>
        public IJoin Join { get; }
        #endregion
    }
}
