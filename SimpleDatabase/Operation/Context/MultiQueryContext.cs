using Database.Types;
using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents initial parameters of a request to query values from several tables.
    /// </summary>
    public interface IMultiQueryContext : IMultiTableConstrainableContext, IQueryContext
    {
        /// <summary>
        ///     Gets the columns specifying values to query.
        /// </summary>
        /// <returns>
        ///     The columns specifying values to query, ordered by tables
        /// </returns>
        IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Filters { get; }
    }

    /// <summary>
    ///     Represents initial parameters of a request to query values from several tables.
    /// </summary>
    public class MultiQueryContext : MultiTableConstrainableContext, IMultiQueryContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiQueryContext"/> class.
        ///     Creating a request with zero columns is valid, and the corresponding operation will always return success with no rows immediately.
        /// </summary>
        /// <parameters>
        /// <param name="join">The join describing how tables are connected in the query.</param>
        /// <param name="filterList">The columns to query.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="join"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filterList"/> is null.
        /// </exception>
        public MultiQueryContext(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> join, IEnumerable<IColumnDescriptor> filterList)
            : base(join)
        {
            InitFilters(filterList);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiQueryContext"/> class.
        ///     Creating a request with zero columns is valid, and the corresponding operation will always return success with no rows immediately.
        /// </summary>
        /// <parameters>
        /// <param name="join">The join describing how tables are connected in the query.</param>
        /// <param name="constraintList">The constraints matching values must fulfill.</param>
        /// <param name="filterList">The columns to query.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="join"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="filterList"/> is null.
        /// </exception>
        public MultiQueryContext(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> join, IEnumerable<IColumnValueCollectionPair> constraintList, IEnumerable<IColumnDescriptor> filterList)
            : base(join, constraintList)
        {
            InitFilters(filterList);
        }

        private void InitFilters(IEnumerable<IColumnDescriptor> filterList)
        {
            Dictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> FilterTable = new Dictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>>();

            foreach (IColumnDescriptor Column in filterList)
            {
                ITableDescriptor Table = Column.Table;

                if (!FilterTable.ContainsKey(Table))
                    FilterTable.Add(Table, new List<IColumnDescriptor>());

                IList<IColumnDescriptor> ColumnFilter = (IList<IColumnDescriptor>)FilterTable[Table];
                ColumnFilter.Add(Column);
            }

            Filters = FilterTable;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the columns specifying values to query.
        /// </summary>
        /// <returns>
        ///     The columns specifying values to query, ordered by tables
        /// </returns>
        public IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Filters { get; private set; }
        #endregion
    }
}
