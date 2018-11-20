using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request on a single table, with constraints.
    /// </summary>
    public interface IMultiRowConstrainableContext : ISingleTableOperationContext
    {
        /// <summary>
        ///     Gets the constraint associated to this request when one column must match one value among many.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        IColumnValueCollectionPair SingleConstraintEntry { get; }

        /// <summary>
        ///     Gets the constraint associated to this request when several columns must match a single value.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        IReadOnlyCollection<IColumnValuePair> MultipleConstraintList { get; }
    }

    /// <summary>
    ///     Represents initial parameters of a request on a single table, with constraints.
    /// </summary>
    public class MultiRowConstrainableContext : SingleTableOperationContext, IMultiRowConstrainableContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiRowConstrainableContext"/> class.
        ///     This request has no constraints.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        public MultiRowConstrainableContext(ITableDescriptor table)
            : base(table)
        {
            SingleConstraintEntry = null;
            MultipleConstraintList = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiRowConstrainableContext"/> class.
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
        public MultiRowConstrainableContext(ITableDescriptor table, IColumnValuePair constraint)
            : base(table)
        {
            if (constraint == null)
                throw new ArgumentNullException(nameof(constraint));

            SingleConstraintEntry = constraint.GetAsCollection();
            MultipleConstraintList = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiRowConstrainableContext"/> class.
        ///     This request has a constraint: one column must match one value among many.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraint">The column and values it must take to match.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is null.
        /// </exception>
        public MultiRowConstrainableContext(ITableDescriptor table, IColumnValueCollectionPair constraint)
            : base(table)
        {
            SingleConstraintEntry = constraint ?? throw new ArgumentNullException(nameof(constraint));
            MultipleConstraintList = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiRowConstrainableContext"/> class.
        ///     This request has a constraint: several columns must match a single value.
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
        public MultiRowConstrainableContext(ITableDescriptor table, IEnumerable<IColumnValuePair> constraintList)
            : base(table)
        {
            SingleConstraintEntry = null;

            if (constraintList != null)
                MultipleConstraintList = new List<IColumnValuePair>(constraintList);
            else
                throw new ArgumentNullException(nameof(constraintList));
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the constraint associated to this request when one column must match one value among many.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        public IColumnValueCollectionPair SingleConstraintEntry { get; }

        /// <summary>
        ///     Gets the constraint associated to this request when several columns must match a single value.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        public IReadOnlyCollection<IColumnValuePair> MultipleConstraintList { get; }
        #endregion
    }
}
