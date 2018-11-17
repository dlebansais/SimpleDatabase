using Database.Types;
using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request that queries values on a single table, with constraints.
    /// </summary>
    public interface ISingleQueryContext : IMultiRowConstrainableContext, IQueryContext
    {
        /// <summary>
        ///     Gets the collection of column values to report.
        /// </summary>
        /// <returns>
        ///     The collection of column values to report.
        /// </returns>
        IReadOnlyCollection<IColumnDescriptor> Filters { get; }
    }

    /// <summary>
    ///     Represents initial parameters of a request that queries values on a single table, with constraints.
    /// </summary>
    public class SingleQueryContext : MultiRowConstrainableContext, ISingleQueryContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleQueryContext"/> class.
        ///     This request has no constraints.
        ///     This request query the value of a single column.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="filter">The column value to report.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filter"/> is null.
        /// </exception>
        public SingleQueryContext(ITableDescriptor table, IColumnDescriptor filter)
            : base(table)
        {
            InitFilters(filter);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleQueryContext"/> class.
        ///     This request has no constraints.
        ///     This request query values of several columns.
        ///     If <paramref name="filters"/> is empty, this query will succeed and report nothing.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="filters">The collection of columns values to report.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filters"/> is null.
        /// </exception>
        public SingleQueryContext(ITableDescriptor table, IEnumerable<IColumnDescriptor> filters)
            : base(table)
        {
            InitFilters(filters);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleQueryContext"/> class.
        ///     This request has a constraint: one column must match one value among many.
        ///     This request query the value of a single column.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraint">The column and values it must take to match.</param>
        /// <param name="filter">The column value to report.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filter"/> is null.
        /// </exception>
        public SingleQueryContext(ITableDescriptor table, IColumnValueCollectionPair constraint, IColumnDescriptor filter)
            : base(table, constraint)
        {
            InitFilters(filter);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleQueryContext"/> class.
        ///     This request has a constraint: one column must match one value among many.
        ///     This request query values of several columns.
        ///     If <paramref name="filters"/> is empty, this query will succeed and report nothing.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraint">The column and values it must take to match.</param>
        /// <param name="filters">The collection of columns values to report.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filters"/> is null.
        /// </exception>
        public SingleQueryContext(ITableDescriptor table, IColumnValueCollectionPair constraint, IEnumerable<IColumnDescriptor> filters)
            : base(table, constraint)
        {
            InitFilters(filters);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleQueryContext"/> class.
        ///     This request has a constraint: several columns must match a single value.
        ///     This request query the value of a single column.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraintList">The columns and value they must take to match.</param>
        /// <param name="filter">The column value to report.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filter"/> is null.
        /// </exception>
        public SingleQueryContext(ITableDescriptor table, IEnumerable<IColumnValuePair> constraintList, IColumnDescriptor filter)
            : base(table, constraintList)
        {
            InitFilters(filter);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleQueryContext"/> class.
        ///     This request has a constraint: several columns must match a single value.
        ///     This request query values of several columns.
        ///     If <paramref name="filters"/> is empty, this query will succeed and report nothing.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table addressed by the request.</param>
        /// <param name="constraintList">The columns and value they must take to match.</param>
        /// <param name="filters">The collection of columns values to report.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filters"/> is null.
        /// </exception>
        public SingleQueryContext(ITableDescriptor table, IEnumerable<IColumnValuePair> constraintList, IEnumerable<IColumnDescriptor> filters)
            : base(table, constraintList)
        {
            InitFilters(filters);
        }

        private void InitFilters(IEnumerable<IColumnDescriptor> filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            Filters = new List<IColumnDescriptor>(filters);
        }

        private void InitFilters(IColumnDescriptor filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            InitFilters(new IColumnDescriptor[] { filter });
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the collection of column values to report.
        /// </summary>
        /// <returns>
        ///     The collection of column values to report.
        /// </returns>
        public IReadOnlyCollection<IColumnDescriptor> Filters { get; private set; }
        #endregion
    }
}
