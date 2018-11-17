using Database.Types;
using System;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents a table in a schema.
    /// </summary>
    public interface ITableDescriptor
    {
        /// <summary>
        ///     Gets the schema.
        /// </summary>
        /// <returns>
        ///     The schema to which the table belongs.
        /// </returns>
        ISchemaDescriptor Schema { get; }

        /// <summary>
        ///     Gets the table name.
        /// </summary>
        /// <returns>
        ///     The table name.
        /// </returns>
        string Name { get; }

        /// <summary>
        ///     Gets the column used to hold primary keys in the table.
        /// </summary>
        /// <returns>
        ///     The column used to hold primary keys in the table.
        /// </returns>
        IColumnDescriptor PrimaryKey { get; }
    }
    #endregion

    /// <summary>
    ///     Represents a table in a schema.
    /// </summary>
    public class TableDescriptor : ITableDescriptor
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableDescriptor"/> class and adds it to the schema it belongs to.
        ///     Columns are added to this table when <see cref="IColumnDescriptor"/> objects are created.
        ///     By convention, the first column added to the table is considered the primary key.
        /// </summary>
        /// <parameters>
        /// <param name="schema">The schema containing the table.</param>
        /// <param name="name">The table name.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="schema"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null or empty.
        /// </exception>
        public TableDescriptor(ISchemaDescriptor schema, string name)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));

            if (Schema is SchemaDescriptor WriteableScheme)
                WriteableScheme.AddTable(this);
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the schema.
        /// </summary>
        /// <returns>
        ///     The schema to which the table belongs.
        /// </returns>
        public ISchemaDescriptor Schema { get; }

        /// <summary>
        ///     Gets the table name.
        /// </summary>
        /// <returns>
        ///     The table name.
        /// </returns>
        public string Name { get; }

        /// <summary>
        ///     Gets the column used to hold primary keys in the table.
        ///     By convention, the first column added to the table is considered the primary key.
        /// </summary>
        /// <returns>
        ///     The column used to hold primary keys in the table.
        /// </returns>
        public virtual IColumnDescriptor PrimaryKey
        {
            get
            {
                if (Schema.Tables.ContainsKey(this))
                    foreach (IColumnDescriptor Column in Schema.Tables[this])
                        return Column;

                return null;
            }
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
