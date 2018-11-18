using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request to delete rows in a table, with constraints on some values.
    /// </summary>
    public interface ISingleRowDeleteContext : ISingleRowConstrainableContext, IModifyContext
    {
    }

    /// <summary>
    ///     Represents initial parameters of a request to delete rows in a table, with constraints on some values.
    /// </summary>
    public class SingleRowDeleteContext : SingleRowConstrainableContext, ISingleRowDeleteContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleRowDeleteContext"/> class.
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
        public SingleRowDeleteContext(ITableDescriptor table, IColumnValuePair constraint)
            : base(table, constraint)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleRowDeleteContext"/> class.
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
        public SingleRowDeleteContext(ITableDescriptor table, IEnumerable<IColumnValuePair> constraintList)
            : base(table, constraintList)
        {
        }
        #endregion
    }
}
