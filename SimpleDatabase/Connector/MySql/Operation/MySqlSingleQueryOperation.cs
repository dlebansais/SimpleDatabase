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

        public virtual string FinalizeOperation(MySqlCommand Command, ISingleQueryResultInternal Result)
        {
            try
            {
                using (MySqlDataReader Reader = Command.EndExecuteReader(Result.AsyncResult))
                {
                    ITableDescriptor Table = Context.Table;
                    ISchemaDescriptor Schema = Table.Schema;
                    IReadOnlyCollection<IColumnDescriptor> TableStructure = Schema.Tables[Table];
                    bool Success = FillResult(Reader, TableStructure, out List<IResultRow> Rows);

                    if (Success)
                    {
                        Result.SetCompletedWithRows(Rows);
                        return $"succeeded, {Rows.Count} row(s) returned";
                    }
                    else
                    {
                        Result.SetCompleted(false);
                        return "failed";
                    }
                }
            }
            catch
            {
                Result.SetCompleted(false);
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

        protected virtual bool GetSingleConstraintString(out string ConstraintString)
        {
            ITableDescriptor Table = Context.Table;
            IColumnValueCollectionPair SingleConstraintEntry = Context.SingleConstraintEntry;

            if (SingleConstraintEntry != null)
            {
                string TableName = Table.Name;
                IColumnDescriptor ConstraintColumn = SingleConstraintEntry.Column;
                string ColumnName = ConstraintColumn.Name;
                IColumnType ColumnType = ConstraintColumn.Type;

                ConstraintString = "";
                foreach (object Value in SingleConstraintEntry.ValueCollection)
                {
                    if (ConstraintString.Length > 0)
                        ConstraintString += " OR ";

                    string FormattedValue = ColumnType.ToSqlFormat(Value);

                    ConstraintString += "(" + ColumnName + "=" + FormattedValue + ")";
                }

                return true;
            }
            else
            {
                ConstraintString = null;
                return false;
            }
        }

        private bool GetMultipleConstraintString(out string ConstraintString)
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValuePair> MultipleConstraintList = Context.MultipleConstraintList;

            string TableName = Table.Name;

            ConstraintString = "";
            foreach (IColumnValuePair Entry in MultipleConstraintList)
            {
                IColumnDescriptor ConstraintColumn = Entry.Column;
                object Value = Entry.Value;
                string ColumnName = ConstraintColumn.Name;
                IColumnType ColumnType = ConstraintColumn.Type;

                if (ConstraintString.Length > 0)
                    ConstraintString += " AND ";

                string FormattedValue = ColumnType.ToSqlFormat(Value);
                ConstraintString += "(" + ColumnName + "=" + FormattedValue + ")";
            }

            return true;
        }

        protected virtual bool GetConstraintString(out string ConstraintString)
        {
            if (Context.MultipleConstraintList != null)
                return GetMultipleConstraintString(out ConstraintString);
            else
                return GetSingleConstraintString(out ConstraintString);
        }

        protected virtual bool FillResult(MySqlDataReader Reader, IReadOnlyCollection<IColumnDescriptor> TableStructure, out List<IResultRow> Rows)
        {
            IReadOnlyCollection<IColumnDescriptor> Filters = Context.Filters;

            if (Filters.Count == 0)
            {
                IList<IColumnDescriptor> MinimalFilters = new List<IColumnDescriptor>();
                foreach (IColumnDescriptor Entry in TableStructure)
                    MinimalFilters.Add(Entry);

                Filters = (IReadOnlyCollection<IColumnDescriptor>)MinimalFilters;
            }

            string TableName = GetTableName();
            Rows = new List<IResultRow>();

            while (Reader.Read())
            {
                IResultRowInternal NewResult = new ResultRow();

                foreach (IColumnDescriptor Column in Filters)
                {
                    string ColumnName = Column.Name;
                    object ConvertedValue = Reader[ColumnName];
                    NewResult.AddResult(Column, ConvertedValue);
                }

                Rows.Add(NewResult);
            }

            return true;
        }
        #endregion
    }
}
