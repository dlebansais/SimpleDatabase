using Database.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database.Internal
{
    internal interface IMySqlUpdateOperation : IUpdateOperation, IMySqlModifyOperation<IUpdateContext, IUpdateResultInternal>
    {
    }

    internal class MySqlUpdateOperation : UpdateOperation, IMySqlUpdateOperation
    {
        #region Init
        public MySqlUpdateOperation(IUpdateContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual string GetCommandText()
        {
            string TableName = GetTableName();
            string ChangeString = GetChangeString();
            string ConstraintString = GetConstraintString();

            string Result = "UPDATE " + TableName + " SET " + ChangeString + " WHERE " + ConstraintString;

            return Result + ";";
        }

        public virtual string FinalizeOperation(MySqlCommand command, IUpdateResultInternal result)
        {
            try
            {
                int ModifiedLines = command.EndExecuteNonQuery(result.AsyncResult);
                if (ModifiedLines > 0)
                {
                    result.SetCompleted(true);
                    return $"succeeded, {ModifiedLines} row(s) modified";
                }
                else
                {
                    result.SetCompleted(false, ResultError.ErrorNoRowModified);
                    return "failed";
                }
            }
            catch
            {
                result.SetCompleted(false, ResultError.ErrorExceptionCompletingUpdate);
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

        protected virtual string GetChangeString()
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValuePair> EntryList = Context.EntryList;
            IReadOnlyCollection<IColumnValuePair<byte[]>> DataEntryList = GetDataEntryList();

            string TableName = Table.Name;

            Debug.Assert(EntryList.Count > 0);

            string ChangeString = "";

            int i = 0;
            foreach (IColumnValuePair<byte[]> DataEntry in DataEntryList)
            {
                if (ChangeString.Length > 0)
                    ChangeString += ", ";

                string ColumnName = DataEntry.Column.Name;
                string DataName = $"data{i}";
                ChangeString += ColumnName + "=" + "?" + DataName;

                i++;
            }

            foreach (IColumnValuePair Entry in EntryList)
            {
                IColumnDescriptor Column = Entry.Column;
                if (Column is IColumnDescriptorByteArray)
                    continue;

                if (ChangeString.Length > 0)
                    ChangeString += ", ";

                string ColumnName = Column.Name;
                IColumnType ColumnType = Column.Type;
                object Value = Entry.Value;
                string FormattedValue = ColumnType.ToSqlFormat(Value);

                ChangeString += ColumnName + "=" + FormattedValue;
            }

            return ChangeString;
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
                IColumnDescriptor Column = Entry.Column;
                object Value = Entry.Value;

                if (ConstraintString.Length > 0)
                    ConstraintString += " AND ";

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
