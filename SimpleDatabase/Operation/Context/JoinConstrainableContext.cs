using Database.Types;
using System;
using System.Collections.Generic;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents initial parameters of a request over several tables, with constraints.
    /// </summary>
    public interface IJoinConstrainableContext : IJoinOperationContext
    {
        /// <summary>
        ///     Gets the constraints associated to this request.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        IReadOnlyDictionary<ITableDescriptor, IColumnValueCollectionPair> Constraints { get; }
    }
    #endregion

    /// <summary>
    ///     Represents initial parameters of a request over several tables, with constraints.
    /// </summary>
    public abstract class JoinConstrainableContext : JoinOperationContext, IJoinConstrainableContext
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinConstrainableContext"/> class.
        ///     This instance has no join and addresses only one table.
        ///     This request has no constraints.
        /// </summary>
        public JoinConstrainableContext()
            : base()
        {
            Constraints = new Dictionary<ITableDescriptor, IColumnValueCollectionPair>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinConstrainableContext"/> class.
        ///     This request has no constraints.
        /// </summary>
        /// <parameters>
        /// <param name="join">The join describing how tables are connected in the request.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="join"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="join"/> does not describe a valid join.
        /// </exception>
        public JoinConstrainableContext(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> join)
            : base(join)
        {
            Constraints = new Dictionary<ITableDescriptor, IColumnValueCollectionPair>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinConstrainableContext"/> class.
        ///     This instance has no join and addresses only one table.
        ///     Each <see cref="IColumnValueCollectionPair"/> in <paramref name="constraintList"/> describes a list of values in a column.
        ///     A row is a match if all columns contain at least one of the listed values.
        /// </summary>
        /// <parameters>
        /// <param name="constraintList">The constraints specifying the subset of values this request must match.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        public JoinConstrainableContext(IEnumerable<IColumnValueCollectionPair> constraintList)
            : base()
        {
            if (constraintList == null)
                throw new ArgumentNullException(nameof(constraintList));
            else
                InitConstraints(constraintList);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinConstrainableContext"/> class.
        ///     Each <see cref="IColumnValueCollectionPair"/> in <paramref name="constraintList"/> describes a list of values in a column.
        ///     A row is a match if all columns contain at least one of the listed values.
        /// </summary>
        /// <parameters>
        /// <param name="join">The join describing how tables are connected in the request.</param>
        /// <param name="constraintList">The constraints specifying the subset of values this request must match.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="join"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="join"/> does not describe a valid join.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintList"/> is null.
        /// </exception>
        public JoinConstrainableContext(IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> join, IEnumerable<IColumnValueCollectionPair> constraintList)
            : base(join)
        {
            if (constraintList == null)
                throw new ArgumentNullException(nameof(constraintList));
            else
                InitConstraints(constraintList);
        }

        private void InitConstraints(IEnumerable<IColumnValueCollectionPair> constraintList)
        {
            Dictionary<ITableDescriptor, IColumnValueCollectionPair> ConstraintTable = new Dictionary<ITableDescriptor, IColumnValueCollectionPair>();

            foreach (IColumnValueCollectionPair Entry in constraintList)
            {
                ITableDescriptor Table = Entry.Column.Table;

                if (!ConstraintTable.ContainsKey(Table))
                    ConstraintTable.Add(Table, Entry);
            }

            Constraints = ConstraintTable;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the constraints associated to this request.
        /// </summary>
        /// <returns>
        ///     The constraints associated to this request.
        /// </returns>
        public IReadOnlyDictionary<ITableDescriptor, IColumnValueCollectionPair> Constraints { get; private set; }
        #endregion
    }
}
