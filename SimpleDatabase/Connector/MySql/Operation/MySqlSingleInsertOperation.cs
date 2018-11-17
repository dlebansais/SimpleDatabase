using Database.Types;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database.Internal
{
    internal interface IMySqlSingleInsertOperation : ISingleInsertOperation, IMySqlModifyOperation<ISingleInsertContext, ISingleInsertResultInternal>
    {
    }

    internal class MySqlSingleInsertOperation : SingleInsertOperation, IMySqlSingleInsertOperation
    {
        #region Init
        public MySqlSingleInsertOperation(ISingleInsertContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual string GetCommandText()
        {
            string TableName = GetTableName();
            string ColumnString;
            string ValueString;
            GetInitialValueString(out ColumnString, out ValueString);

            string Result = "INSERT INTO " + TableName + " ( " + ColumnString + " ) VALUES ( " + ValueString + " )";

            return Result + ";";
        }

        public virtual string FinalizeOperation(MySqlCommand Command, ISingleInsertResultInternal Result)
        {
            IColumnValuePair CreatedKeyId = null;
            bool Success;
            string Diagnostic;

            int InsertedLines = Command.EndExecuteNonQuery(Result.AsyncResult);
            if (InsertedLines > 0)
            {
                CreatedKeyId = GetCreatedKeyId(Command, Result);
                Diagnostic = $"succeeded, {InsertedLines} row(s) inserted";
                Success = true;
            }
            else
            {
                Diagnostic = "failed";
                Success = false;
            }

            Result.SetCompletedWithId(Success, CreatedKeyId);
            return Diagnostic;
        }
        #endregion

        #region Implementation
        protected virtual string GetTableName()
        {
            ITableDescriptor Table = Context.Table;

            return Table.Name;
        }

        protected virtual void GetInitialValueString(out string ColumnString, out string ValueString)
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValuePair> EntryList = Context.EntryList;
            IReadOnlyCollection<IColumnValuePair<byte[]>> DataEntryList = GetDataEntryList();

            Debug.Assert(EntryList.Count > 0);

            string TableName = GetTableName();

            ColumnString = "";
            ValueString = "";

            int i = 0;
            foreach (IColumnValuePair<byte[]> DataEntry in DataEntryList)
            {
                if (ColumnString.Length > 0)
                    ColumnString += ", ";
                if (ValueString.Length > 0)
                    ValueString += ", ";

                string DataName = $"data{i}";
                string ColumnName = DataEntry.Column.Name;

                ColumnString += ColumnName;
                ValueString += "?" + DataName;

                i++;
            }

            foreach (IColumnValuePair Entry in EntryList)
            {
                IColumnDescriptor Column = Entry.Column;
                if (Column is IColumnDescriptorByteArray)
                    continue;

                if (ColumnString.Length > 0)
                    ColumnString += ", ";
                if (ValueString.Length > 0)
                    ValueString += ", ";

                string ColumnName = Column.Name;
                IColumnType ColumnType = Column.Type;
                string Value = ColumnType.ToSqlFormat(Entry.Value);

                ColumnString += ColumnName;
                ValueString += Value;
            }
        }

        protected virtual IColumnValuePair GetCreatedKeyId(MySqlCommand command, ISingleInsertResultInternal result)
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValuePair> ColumnInitialValues = Context.EntryList;
            IColumnDescriptor PrimaryKeyColumn = Table.PrimaryKey;

            if (PrimaryKeyColumn is IColumnDescriptorInt AsIntColumn)
                return new ColumnValuePair<int>(AsIntColumn, (int)command.LastInsertedId);

            foreach (IColumnValuePair Entry in ColumnInitialValues)
                if (Entry.Column == PrimaryKeyColumn)
                    return Entry;

            Debug.Assert(false);
            return null;
        }
        #endregion
    }
}
