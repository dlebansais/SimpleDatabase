using Database.Types;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Database.Internal
{
    internal interface IMySqlSingleQueryOperation : ISingleQueryOperation, IMySqlQueryOperation<ISingleQueryContext, ISingleQueryResultInternal>
    {
    }

    internal class MySqlSingleQueryOperation : SingleQueryOperation, IMySqlSingleQueryOperation
    {
        #region Init
        public MySqlSingleQueryOperation(ISingleQueryContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual string GetCommandText()
        {
            string FilterString = GetFilterString();
            string TableName = GetTableName();

            string Result = "SELECT " + FilterString + " FROM " + TableName;

            string ConstraintString;
            if (GetConstraintString(out ConstraintString))
                Result += " WHERE " + ConstraintString;

            return Result + ";";
        }

        public virtual string FinalizeOperation(MySqlCommand command, ISingleQueryResultInternal result)
        {
            try
            {
                using (MySqlDataReader Reader = command.EndExecuteReader(result.AsyncResult))
                {
                    ITableDescriptor Table = Context.Table;
                    ISchemaDescriptor Schema = Table.Schema;
                    IReadOnlyCollection<IColumnDescriptor> TableStructure = Schema.Tables[Table];
                    FillResult(Reader, TableStructure, out List<IResultRow> Rows);

                    result.SetCompletedWithRows(Rows);
                    return $"succeeded, {Rows.Count} row(s) returned";
                }
            }
            catch
            {
                result.SetCompleted(false, ResultError.ErrorExceptionCompletingQuery);
                throw;
            }
        }
        #endregion

        #region Implementation
        protected virtual string GetFilterString()
        {
            IReadOnlyCollection<IColumnDescriptor> Filters = Context.Filters;

            if (Filters.Count > 0)
            {
                string TableName = GetTableName();
                string FilterString = "";

                foreach (IColumnDescriptor Column in Filters)
                {
                    if (FilterString.Length > 0)
                        FilterString += ", ";

                    string ColumnName = Column.Name;

                    FilterString += ColumnName;
                }

                return FilterString;
            }
            else
                return "*";
        }

        protected virtual string GetTableName()
        {
            ITableDescriptor Table = Context.Table;
            return Table.Name;
        }

        protected virtual bool GetSingleConstraintString(out string constraintString)
        {
            ITableDescriptor Table = Context.Table;
            IColumnValueCollectionPair SingleConstraintEntry = Context.SingleConstraintEntry;

            if (SingleConstraintEntry != null)
            {
                string TableName = Table.Name;
                IColumnDescriptor ConstraintColumn = SingleConstraintEntry.Column;
                string ColumnName = ConstraintColumn.Name;
                IColumnType ColumnType = ConstraintColumn.Type;

                constraintString = "";
                foreach (object Value in SingleConstraintEntry.ValueCollection)
                {
                    if (constraintString.Length > 0)
                        constraintString += " OR ";

                    string FormattedValue = ColumnType.ToSqlFormat(Value);

                    constraintString += "(" + ColumnName + "=" + FormattedValue + ")";
                }

                return true;
            }
            else
            {
                constraintString = null;
                return false;
            }
        }

        private bool GetMultipleConstraintString(out string constraintString)
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValuePair> MultipleConstraintList = Context.MultipleConstraintList;

            string TableName = Table.Name;

            constraintString = "";
            foreach (IColumnValuePair Entry in MultipleConstraintList)
            {
                IColumnDescriptor ConstraintColumn = Entry.Column;
                object Value = Entry.Value;
                string ColumnName = ConstraintColumn.Name;
                IColumnType ColumnType = ConstraintColumn.Type;

                if (constraintString.Length > 0)
                    constraintString += " AND ";

                string FormattedValue = ColumnType.ToSqlFormat(Value);
                constraintString += "(" + ColumnName + "=" + FormattedValue + ")";
            }

            return true;
        }

        protected virtual bool GetConstraintString(out string constraintString)
        {
            if (Context.MultipleConstraintList != null)
                return GetMultipleConstraintString(out constraintString);
            else
                return GetSingleConstraintString(out constraintString);
        }

        protected virtual void FillResult(MySqlDataReader reader, IReadOnlyCollection<IColumnDescriptor> tableStructure, out List<IResultRow> rows)
        {
            IReadOnlyCollection<IColumnDescriptor> Filters = Context.Filters;

            if (Filters.Count == 0)
            {
                IList<IColumnDescriptor> MinimalFilters = new List<IColumnDescriptor>();
                foreach (IColumnDescriptor Entry in tableStructure)
                    MinimalFilters.Add(Entry);

                Filters = (IReadOnlyCollection<IColumnDescriptor>)MinimalFilters;
            }

            string TableName = GetTableName();
            rows = new List<IResultRow>();

            while (reader.Read())
            {
                IResultRowInternal NewResult = new ResultRow();

                foreach (IColumnDescriptor Column in Filters)
                {
                    string ColumnName = Column.Name;
                    int ColumnIndex = reader.GetOrdinal(ColumnName);
                    if (!reader.IsDBNull(ColumnIndex))
                        NewResult.AddResult(Column, reader[ColumnName]);
                }

                rows.Add(NewResult);
            }
        }
        #endregion
    }
}
