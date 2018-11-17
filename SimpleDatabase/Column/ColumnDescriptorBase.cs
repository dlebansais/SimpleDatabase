using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database.Types
{
    #region Interface
    /// <summary>
    ///     Represents a column, in a table of a schema, containing any type of value.
    /// </summary>
    public interface IColumnDescriptorBase : IColumnDescriptor
    {
    }

    /// <summary>
    ///     Represents a column, in a table of a schema, containing values of type <typeparamref name="T"/>.
    /// </summary>
    public interface IColumnDescriptorBase<T> : IColumnDescriptorBase, IColumnDescriptor<T>
    {
    }
    #endregion

    /// <summary>
    ///     Represents a column, in a table of a schema, containing any type of value.
    /// </summary>
    public abstract class ColumnDescriptorBase : IColumnDescriptorBase
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescriptorBase"/> class and adds it to the table it belongs to.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table containing the column.</param>
        /// <param name="name">The column name.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null or empty.
        /// </exception>
        public ColumnDescriptorBase(ITableDescriptor table, string name)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));

            if ((table is TableDescriptor WriteableTable) && (table.Schema is SchemaDescriptor WriteableSchema))
            {
                Debug.Assert(WriteableSchema.Tables.ContainsKey(table));

                if (WriteableSchema.Tables.ContainsKey(table))
                {
                    IList<IColumnDescriptor> Columns = WriteableSchema.Tables[table] as IList<IColumnDescriptor>;
                    Debug.Assert(Columns != null);

                    Columns?.Add(this);
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the table.
        /// </summary>
        /// <returns>
        ///     The table to which the column belongs.
        /// </returns>
        public ITableDescriptor Table { get; }

        /// <summary>
        ///     Gets the column name.
        /// </summary>
        /// <returns>
        ///     The column name.
        /// </returns>
        public string Name { get; }

        /// <summary>
        ///     Gets the column type.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnType"/> object that represents the type of values the column can hold.
        /// </returns>
        public abstract IColumnType Type { get; }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Associates a value with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValuePair"/> representing the column and value pair.
        /// </returns>
        public abstract IColumnValuePair CreateValuePair(object value);

        /// <summary>
        ///     Associates a collection of values with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="collection">The collection of values to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValueCollectionPair"/> representing the column and collection of values pair.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="collection"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="collection"/> is a collection of items of the wrong type.
        /// </exception>
        public abstract IColumnValueCollectionPair CreateValueCollectionPair(IEnumerable collection);
        #endregion
    }

    /// <summary>
    ///     Represents a column, in a table of a schema, containing values of type <typeparamref name="T"/>.
    /// </summary>
    public abstract class ColumnDescriptorBase<T> : ColumnDescriptorBase, IColumnDescriptorBase<T>
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescriptorBase{T}"/> class and adds it to the table it belongs to.
        /// </summary>
        /// <parameters>
        /// <param name="table">The table containing the column.</param>
        /// <param name="name">The column name.</param>
        /// <param name="type">The type of values in the column.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="table"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="type"/> is null.
        /// </exception>
        public ColumnDescriptorBase(ITableDescriptor table, string name, IColumnType<T> type)
            : base(table, name)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the column type.
        /// </summary>
        /// <returns>
        ///     A <see cref="IColumnType"/> object that represents the type of values the column can hold.
        /// </returns>
        public override IColumnType Type { get; }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Try to parse a row containing a value for this column.
        /// </summary>
        /// <parameters>
        /// <param name="row">The row to parse.</param>
        /// <param name="value">The value of type <typeparamref name="T"/>, if parsed successfully.</param>
        /// </parameters>
        /// <returns>
        ///     True if parsed successfully, False otherwise and in that case <paramref name="value"/> is unspecified.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="row"/> is null.
        /// </exception>
        public bool TryParseRow(IResultRow row, out T value)
        {
            IResultRowInternal RecastRow = row as IResultRowInternal;
            IColumnType<T> RecastType = Type as IColumnType<T>;

            Debug.Assert(RecastRow != null);
            Debug.Assert(RecastType != null);
            if (RecastRow != null && RecastType != null)
            {
                IDictionary<IColumnDescriptor, object> ColumnTable = RecastRow.ColumnTable;

                Debug.Assert(ColumnTable != null);
                if (ColumnTable != null)
                {
                    if (ColumnTable.ContainsKey(this))
                    {
                        value = RecastType.FromSqlFormat(ColumnTable[this]);
                        return true;
                    }
                }
            }

            value = default(T);
            return false;
        }

        /// <summary>
        ///     Associates a value with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value of type <typeparamref name="T"/> to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValuePair{T}"/> representing the column and value pair.
        /// </returns>
        public IColumnValuePair<T> CreateValuePair(T value)
        {
            return new ColumnValuePair<T>(this, value);
        }

        /// <summary>
        ///     Associates a value with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="value">The value to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValuePair"/> representing the column and value pair.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="value"/> is of the wrong type.
        /// </exception>
        public override IColumnValuePair CreateValuePair(object value)
        {
            if (value is T)
            {
                IColumnValuePair Result = CreateValuePair((T)value) as IColumnValuePair;

                Debug.Assert(Result != null);
                return Result;
            }
            else
                throw new ArgumentException(nameof(value));
        }

        /// <summary>
        ///     Associates a collection of values with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="collection">The collection of values of type <typeparamref name="T"/> to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValueCollectionPair{T}"/> representing the column and collection of values pair.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="collection"/> is null.
        /// </exception>
        public IColumnValueCollectionPair<T> CreateValueCollectionPair(IEnumerable<T> collection)
        {
            return new ColumnValueCollectionPair<T>(this, collection);
        }

        /// <summary>
        ///     Associates a collection of values with the column to create a pair.
        /// </summary>
        /// <parameters>
        /// <param name="collection">The collection of values to use to create the pair.</param>
        /// </parameters>
        /// <returns>
        ///     A <see cref="IColumnValueCollectionPair"/> representing the column and collection of values pair.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="collection"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="collection"/> is a collection of items of the wrong type.
        /// </exception>
        public override IColumnValueCollectionPair CreateValueCollectionPair(IEnumerable collection)
        {
            IEnumerable<T> RecastCollection = (collection as IEnumerable<T> ?? throw new ArgumentException(nameof(collection))) ?? throw new ArgumentNullException(nameof(collection));
            IColumnValueCollectionPair Result = CreateValueCollectionPair(RecastCollection) as IColumnValueCollectionPair;

            Debug.Assert(Result != null);
            return Result;
        }
        #endregion

        #region Debugging
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return Name;
        }
        #endregion
    }
}
