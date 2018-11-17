using Database.Types;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database.Internal
{
    internal interface IMySqlMultiRowDeleteOperation : IMultiRowDeleteOperation, IMySqlModifyOperation<IMultiRowDeleteContext, IMultiRowDeleteResultInternal>
    {
    }

    internal class MySqlMultiRowDeleteOperation : MultiRowDeleteOperation, IMySqlMultiRowDeleteOperation
    {
        #region Init
        public MySqlMultiRowDeleteOperation(IMultiRowDeleteContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual string GetCommandText()
        {
            string TableName = GetTableName();
            string ConstraintString = GetConstraintString();

            string Result = "DELETE FROM " + TableName + " WHERE " + ConstraintString;

            return Result + ";";
        }

        public virtual string FinalizeOperation(MySqlCommand Command, IMultiRowDeleteResultInternal Result)
        {
            int DeletedLines = Command.EndExecuteNonQuery(Result.AsyncResult);
            int MinRowDelete = Context.ExpectedDeletedCount;
            bool Success = (DeletedLines >= MinRowDelete);
            Result.SetCompleted(Success);

            if (Success)
                return $"succeeded, {DeletedLines} row(s) deleted";
            else
                return "failed";
        }
        #endregion

        #region Implementation
        protected virtual string GetTableName()
        {
            ITableDescriptor Table = Context.Table;
            return Table.Name;
        }

        private string GetSingleConstraintString()
        {
            ITableDescriptor Table = Context.Table;
            IColumnValueCollectionPair SingleConstraintEntry = Context.SingleConstraintEntry;

            Debug.Assert(SingleConstraintEntry != null);

            string TableName = Table.Name;
            IColumnDescriptor ConstraintColumn = SingleConstraintEntry.Column;
            string ColumnName = ConstraintColumn.Name;
            IColumnType ColumnType = ConstraintColumn.Type;

            string ConstraintString = "";
            foreach (object Value in SingleConstraintEntry.ValueCollection)
            {
                if (ConstraintString.Length > 0)
                    ConstraintString += " OR ";

                string FormattedValue = ColumnType.ToSqlFormat(Value);

                ConstraintString += "(" + ColumnName + "=" + FormattedValue + ")";
            }

            return ConstraintString;
        }

        private string GetMultipleConstraintString()
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValuePair> MultipleConstraintList = Context.MultipleConstraintList;

            string TableName = Table.Name;

            string ConstraintString = "";
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

            return ConstraintString;
        }

        protected virtual string GetConstraintString()
        {
            if (Context.MultipleConstraintList != null)
                return GetMultipleConstraintString();
            else
                return GetSingleConstraintString();
        }
        #endregion
    }
}
