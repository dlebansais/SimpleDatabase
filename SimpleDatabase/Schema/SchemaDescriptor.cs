using Database.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents a schema, the root for table.
    /// </summary>
    public interface ISchemaDescriptor
    {
        /// <summary>
        ///     Gets the schema name.
        /// </summary>
        /// <returns>
        ///     The schema name.
        /// </returns>
        string Name { get; }

        /// <summary>
        ///     Gets the tables belonging to this schema.
        /// </summary>
        /// <returns>
        ///     The tables belonging to this schema.
        /// </returns>
        IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Tables { get; }

        /// <summary>
        ///     Indicates if the DateTime type is stored as a tick count.
        /// </summary>
        /// <returns>
        ///     True if the DateTime type is stored as a tick count.
        /// </returns>
        bool DateTimeAsTicks { get; }
    }
    #endregion

    /// <summary>
    ///     Represents a schema, the root for table.
    /// </summary>
    public class SchemaDescriptor : ISchemaDescriptor
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaDescriptor"/> class.
        ///     Tables are added to this schema when <see cref="ITableDescriptor"/> objects are created.
        /// </summary>
        /// <parameters>
        /// <param name="name">The schema name.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null or empty.
        /// </exception>
        public SchemaDescriptor(string name)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            WriteableTables = new Dictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>>();
            Tables = new ReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>>(WriteableTables);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaDescriptor"/> class.
        ///     Tables are added to this schema when <see cref="ITableDescriptor"/> objects are created.
        /// </summary>
        /// <parameters>
        /// <param name="name">The schema name.</param>
        /// <param name="dateTimeAsTicks">Indicates if the DateTime type is stored as a tick count.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null or empty.
        /// </exception>
        public SchemaDescriptor(string name, bool dateTimeAsTicks)
            : this(name)
        {
            DateTimeAsTicks = dateTimeAsTicks;
        }

        internal void AddTable(ITableDescriptor table)
        {
            WriteableTables.Add(table, new List<IColumnDescriptor>());
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the schema name.
        /// </summary>
        /// <returns>
        ///     The schema name.
        /// </returns>
        public string Name { get; }

        /// <summary>
        ///     Gets the tables belonging to this schema.
        /// </summary>
        /// <returns>
        ///     The tables belonging to this schema.
        /// </returns>
        public IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Tables { get; }
        private IDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> WriteableTables;

        /// <summary>
        ///     Indicates if the DateTime type is stored as a tick count.
        /// </summary>
        /// <returns>
        ///     True if the DateTime type is stored as a tick count.
        /// </returns>
        public bool DateTimeAsTicks { get; }
        #endregion
    }
}
