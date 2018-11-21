using System;
using System.Collections.Generic;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents initial parameters of a request over only one table, with constraints on some values.
    /// </summary>
    public interface ISingleRowConstrainableContext : ISingleTableOperationContext
    {
        /// <summary>
        ///     Gets the constraint associated to this request, where several columns must match a single value.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        IReadOnlyCollection<IColumnValuePair> ConstraintList { get; }
    }
    #endregion

    /// <summary>
    ///     Represents initial parameters of a request over only one table, with constraints on some values.
    /// </summary>
    public class SingleRowConstrainableContext : SingleTableOperationContext, ISingleRowConstrainableContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleRowConstrainableContext"/> class.
        ///     This request has a constraint: one column must match one value.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraint">The column and value it must take.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is null.
        /// </exception>
        public SingleRowConstrainableContext(ITableDescriptor table, IColumnValuePair constraint)
            : base(table)
        {
            if (constraint == null)
                throw new ArgumentNullException(nameof(constraint));

            ConstraintList = new List<IColumnValuePair>() { constraint };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleRowConstrainableContext"/> class.
        ///     This request has a constraint: several columns must match a given value.
        ///     If <paramref name="constraintList"/> is empty, no constraint will be applied to the request.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraintList">The columns and value they must take to match.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        public SingleRowConstrainableContext(ITableDescriptor table, IEnumerable<IColumnValuePair> constraintList)
            : base(table)
        {
            if (constraintList == null)
                throw new ArgumentNullException(nameof(constraintList));

            ConstraintList = new List<IColumnValuePair>(constraintList);
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the constraint associated to this request, where several columns must match a single value.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        public IReadOnlyCollection<IColumnValuePair> ConstraintList { get; }
        #endregion
    }
}
