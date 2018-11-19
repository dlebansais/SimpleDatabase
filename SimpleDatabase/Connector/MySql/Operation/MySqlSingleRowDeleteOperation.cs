using Database.Types;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database.Internal
{
    internal interface IMySqlSingleRowDeleteOperation : ISingleRowDeleteOperation, IMySqlModifyOperation<ISingleRowDeleteContext, ISingleRowDeleteResultInternal>
    {
    }

    internal class MySqlSingleRowDeleteOperation : SingleRowDeleteOperation, IMySqlSingleRowDeleteOperation
    {
        #region Init
        public MySqlSingleRowDeleteOperation(ISingleRowDeleteContext context)
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

        public virtual string FinalizeOperation(MySqlCommand Command, ISingleRowDeleteResultInternal Result)
        {
            try
            {
                int DeletedLines = Command.EndExecuteNonQuery(Result.AsyncResult);
                bool Success = DeletedLines > 0;
                Result.SetCompleted(Success);

                if (Success)
                    return $"succeeded, {DeletedLines} row(s) deleted";
                else
                    return "failed";
            }
            catch
            {
                Result.SetCompleted(false);
                throw;
            }
        }
        #endregion

        #region Implementation
        protected virtual string GetTableName()
        {
            ITableDescriptor Table = Context.Table;
            return Table.Name;
        }

        protected virtual string GetConstraintString()
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValuePair> ConstraintList = Context.ConstraintList;

            Debug.Assert(ConstraintList != null);
            Debug.Assert(ConstraintList.Count > 0);

            string TableName = Table.Name;

            string ConstraintString = "";
            foreach (IColumnValuePair Entry in ConstraintList)
            {
                if (ConstraintString.Length > 0)
                    ConstraintString += " AND ";

                IColumnDescriptor Column = Entry.Column;
                object Value = Entry.Value;
                string ColumnName = Column.Name;

                IColumnType ColumnType = Column.Type;
                string FormattedValue = ColumnType.ToSqlFormat(Value);

                ConstraintString += "(" + ColumnName + "=" + FormattedValue + ")";
            }

            Debug.Assert(!string.IsNullOrEmpty(ConstraintString));
            return ConstraintString;
        }
        #endregion
    }
}
