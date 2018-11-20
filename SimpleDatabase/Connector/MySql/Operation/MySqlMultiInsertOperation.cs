using Database.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database.Internal
{
    internal interface IMySqlMultiInsertOperation : IMultiInsertOperation, IMySqlModifyOperation<IMultiInsertContext, IMultiInsertResultInternal>
    {
    }

    internal class MySqlMultiInsertOperation : MultiInsertOperation, IMySqlMultiInsertOperation
    {
        #region Init
        public MySqlMultiInsertOperation(IMultiInsertContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual string GetCommandText()
        {
            string TableName = GetTableName();

            string Result = $"INSERT INTO {TableName}";

            string ColumnString;
            ICollection<string> ValueStringList;
            GetInitialValueString(out ColumnString, out ValueStringList);
            Debug.Assert(!string.IsNullOrEmpty(ColumnString));
            Debug.Assert(ValueStringList.Count > 0);

            string Values = "";
            foreach (string ValueString in ValueStringList)
            {
                if (Values.Length > 0)
                    Values += ", ";

                Values += $"( {ValueString} )";
            }

            Result += $" ( {ColumnString} ) VALUES {Values};";

            return Result;
        }

        public virtual string FinalizeOperation(MySqlCommand Command, IMultiInsertResultInternal Result)
        {
            IColumnValuePair LastCreatedKeyId = null;
            string Diagnostic;

            try
            {
                int InsertedLines = Command.EndExecuteNonQuery(Result.AsyncResult);

                if (InsertedLines > 0)
                {
                    LastCreatedKeyId = GetLastCreatedKeyId(Command, Result);
                    Result.SetCompletedWithId(LastCreatedKeyId);
                    Diagnostic = $"succeeded, {InsertedLines} rows inserted";
                }
                else
                {
                    Result.SetCompleted(false);
                    Diagnostic = "failed";
                }

                return Diagnostic;
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

        protected virtual void GetInitialValueString(out string ColumnString, out ICollection<string> ValueStringList)
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValueCollectionPair> EntryList = Context.EntryList;
            IReadOnlyCollection<IColumnValueCollectionPair<byte[]>> DataEntryList = GetDataEntryList();
            int RowCount = Context.RowCount;

            Debug.Assert(EntryList.Count > 0);
            string TableName = GetTableName();

            ColumnString = "";
            string[] StringList = new string[RowCount];
            for (int i = 0; i < RowCount; i++)
                StringList[i] = "";

            int DataCount = 0;
            foreach (IColumnValueCollectionPair<byte[]> DataEntry in DataEntryList)
            {
                IColumnDescriptor Column = DataEntry.Column;
                IColumnType ColumnType = Column.Type;
                IEnumerable ValueCollection = DataEntry.ValueCollection;
                string ColumnName = Column.Name;

                if (ColumnString.Length > 0)
                    ColumnString += ", ";
                ColumnString += ColumnName;

                int i = 0;
                foreach (object Value in ValueCollection)
                    if (i < RowCount)
                    {
                        string ValueString = StringList[i];
                        if (ValueString.Length > 0)
                            ValueString += ", ";

                        ValueString += $"?data{DataCount}";
                        StringList[i] = ValueString;

                        DataCount++;
                        i++;
                    }

                Debug.Assert(i == RowCount);
            }

            foreach (IColumnValueCollectionPair Entry in EntryList)
            {
                IColumnDescriptor Column = Entry.Column;
                if (Column is IColumnDescriptorByteArray)
                    continue;

                IColumnType ColumnType = Column.Type;
                IEnumerable ValueCollection = Entry.ValueCollection;
                string ColumnName = Column.Name;

                if (ColumnString.Length > 0)
                    ColumnString += ", ";
                ColumnString += ColumnName;

                int i = 0;
                foreach (object Value in ValueCollection)
                    if (i < RowCount)
                    {
                        string ValueString = StringList[i];
                        if (ValueString.Length > 0)
                            ValueString += ", ";

                        ValueString += ColumnType.ToSqlFormat(Value);
                        StringList[i] = ValueString;

                        i++;
                    }

                Debug.Assert(i == RowCount);
            }

            ValueStringList = StringList;
        }

        protected virtual IColumnValuePair GetLastCreatedKeyId(MySqlCommand command, IMultiInsertResultInternal result)
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValueCollectionPair> ColumnInitialValues = Context.EntryList;
            IColumnDescriptor PrimaryKeyColumn = Table.PrimaryKey;

            if (PrimaryKeyColumn is IColumnDescriptorInt AsIntColumn)
                return new ColumnValuePair<int>(AsIntColumn, (int)command.LastInsertedId);

            foreach (IColumnValueCollectionPair Entry in ColumnInitialValues)
                if (Entry.Column == PrimaryKeyColumn)
                    return Entry.LastEntry;

            Debug.Assert(false);
            return null;
        }
        #endregion
    }
}
