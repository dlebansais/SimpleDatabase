﻿using Database.Types;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Database.Internal
{
    internal interface IMySqlJoinQueryOperation : IJoinQueryOperation, IMySqlQueryOperation<IJoinQueryContext, IJoinQueryResultInternal>
    {
    }

    internal class MySqlJoinQueryOperation : JoinQueryOperation, IMySqlJoinQueryOperation
    {
        #region Init
        public MySqlJoinQueryOperation(IJoinQueryContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public string GetCommandText()
        {
            string FilterString = GetFilterString();
            string TableReference = GetTableReference();

            string Result = "SELECT " + FilterString + " FROM " + TableReference;

            string ConstraintString;
            if (GetConstraintString(out ConstraintString))
                Result += " WHERE " + ConstraintString;

            return Result + ";";
        }

        public virtual string FinalizeOperation(MySqlCommand command, IJoinQueryResultInternal result)
        {
            try
            {
                using (MySqlDataReader Reader = command.EndExecuteReader(result.AsyncResult))
                {
                    FillResult(Reader, out List<IResultRow> Rows);
                    result.SetCompletedWithRows(Rows);
                    return $"succeeded, {Rows.Count} row(s) returned";
                }
            }
            catch
            {
                result.SetCompleted(false, ResultError.ErrorExceptionCompletingJoinQuery);
                throw;
            }
        }
        #endregion

        #region Implementation
        protected virtual string GetFilterString()
        {
            IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Filters = Context.Filters;
            IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> Join = Context.Join.Columns;

            List<ITableDescriptor> TableList = new List<ITableDescriptor>();
            foreach (KeyValuePair<IColumnDescriptor, IColumnDescriptor> Entry in Join)
            {
                ITableDescriptor Table;

                Table = Entry.Key.Table;
                if (!TableList.Contains(Table))
                    TableList.Add(Table);

                Table = Entry.Value.Table;
                if (!TableList.Contains(Table))
                    TableList.Add(Table);
            }

            if (Filters.Count == 0)
            {
                IDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> MinimalFilters = new Dictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>>();
                foreach (ITableDescriptor Table in TableList)
                {
                    IReadOnlyCollection<IColumnDescriptor> TableStructure = Table.Schema.Tables[Table];
                    MinimalFilters.Add(Table, TableStructure);
                }

                Filters = (IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>>)MinimalFilters;
            }

            string FilterString = "";

            foreach (KeyValuePair<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Entry in Filters)
            {
                ITableDescriptor Table = Entry.Key;
                IEnumerable<IColumnDescriptor> ColumnFilter = Entry.Value;

                string TableName = Table.Name;

                foreach (IColumnDescriptor Column in ColumnFilter)
                {
                    if (FilterString.Length > 0)
                        FilterString += ", ";

                    string ColumnName = Column.Name;

                    FilterString += TableName + "." + ColumnName + " AS " + TableName + "_" + ColumnName;
                }
            }

            return FilterString;
        }

        protected virtual string GetTableReference()
        {
            IReadOnlyDictionary<IColumnDescriptor, IColumnDescriptor> Join = Context.Join.Columns;

            string JoinResult = "";
            foreach (KeyValuePair<IColumnDescriptor, IColumnDescriptor> Entry in Join)
            {
                IColumnDescriptor LeftColumn = Entry.Key;
                ITableDescriptor LeftTable = LeftColumn.Table;
                string LeftTableName = LeftTable.Name;
                string LeftColumnName = LeftColumn.Name;

                IColumnDescriptor RightColumn = Entry.Value;
                ITableDescriptor RightTable = RightColumn.Table;
                string RightTableName = RightTable.Name;
                string RightColumnName = RightColumn.Name;

                if (JoinResult.Length == 0)
                    JoinResult = RightTableName;

                JoinResult += " LEFT JOIN " + LeftTableName + " ON " + LeftTableName + "." + LeftColumnName + "=" + RightTableName + "." + RightColumnName;
            }

            if (JoinResult.Length == 0)
            {
                IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Filters = Context.Filters;

                foreach (KeyValuePair<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Entry in Filters)
                {
                    ITableDescriptor Table = Entry.Key;
                    string TableName = Table.Name;

                    JoinResult = TableName;
                    break;
                }
            }

            return JoinResult;
        }

        protected virtual bool GetConstraintString(out string constraintString)
        {
            IReadOnlyDictionary<ITableDescriptor, IColumnValueCollectionPair> Constraints = Context.Constraints;

            if (Constraints.Count > 0)
            {
                constraintString = "";
                foreach (KeyValuePair<ITableDescriptor, IColumnValueCollectionPair> Entry in Constraints)
                {
                    ITableDescriptor Table = Entry.Key;
                    IColumnDescriptor Column = Entry.Value.Column;
                    IEnumerable ValueList = Entry.Value.ValueCollection;

                    string TableName = Table.Name;
                    string ColumnName = Column.Name;

                    foreach (object Value in ValueList)
                    {
                        if (constraintString.Length > 0)
                            constraintString += " OR ";

                        IColumnType ColumnType = Column.Type;
                        string FormattedValue = ColumnType.ToSqlFormat(Value);

                        constraintString += "(" + TableName + "." + ColumnName + "=" + FormattedValue + ")";
                    }
                }

                return true;
            }

            else
            {
                constraintString = null;
                return false;
            }
        }

        protected virtual void FillResult(MySqlDataReader reader, out List<IResultRow> rows)
        {
            IReadOnlyDictionary<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Filters = Context.Filters;

            rows = new List<IResultRow>();
            while (reader.Read())
            {
                IResultRowInternal NewResult = new ResultRow();

                foreach (KeyValuePair<ITableDescriptor, IReadOnlyCollection<IColumnDescriptor>> Entry in Filters)
                {
                    ITableDescriptor Table = Entry.Key;
                    IEnumerable<IColumnDescriptor> ColumnFilter = Entry.Value;

                    foreach (IColumnDescriptor Column in ColumnFilter)
                    {
                        string TableName = Table.Name;
                        string ColumnName = Column.Name;
                        string ResultName = TableName + "_" + ColumnName;

                        int Index = reader.GetOrdinal(ResultName);
                        if (reader.IsDBNull(Index))
                            continue;

                        object ConvertedValue = reader[ResultName];
                        NewResult.AddResult(Column, ConvertedValue);
                    }
                }

                rows.Add(NewResult);
            }
        }
        #endregion
    }
}
