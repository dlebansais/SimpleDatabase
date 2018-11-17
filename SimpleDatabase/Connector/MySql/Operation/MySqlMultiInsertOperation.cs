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

            string Result = "INSERT INTO " + TableName;

            string ColumnString;
            IEnumerable<string> ValueStringList;
            if (GetInitialValueString(out ColumnString, out ValueStringList))
            {
                string Values = "";
                foreach (string ValueString in ValueStringList)
                {
                    if (Values.Length > 0)
                        Values += ", ";

                    Values += "( " + ValueString + " )";
                }

                Result += " ( " + ColumnString + " ) VALUES " + Values;
            }

            return Result + ";";
        }

        public virtual string FinalizeOperation(MySqlCommand Command, IMultiInsertResultInternal Result)
        {
            int InsertedLines = Command.EndExecuteNonQuery(Result.AsyncResult);
            bool Success = (InsertedLines > 0);
            Result.SetCompleted(Success);

            if (Success)
                return $"succeeded, {InsertedLines} row(s) inserted";
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

        protected virtual bool GetInitialValueString(out string ColumnString, out IEnumerable<string> ValueStringList)
        {
            ITableDescriptor Table = Context.Table;
            IReadOnlyCollection<IColumnValueCollectionPair> EntryList = Context.EntryList;
            int RowCount = Context.RowCount;

            Debug.Assert(EntryList.Count > 0);
            string TableName = GetTableName();

            ColumnString = "";
            string[] StringList = new string[RowCount];
            for (int i = 0; i < RowCount; i++)
                StringList[i] = "";

            foreach (IColumnValueCollectionPair Entry in EntryList)
            {
                IColumnDescriptor Column = Entry.Column;
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

                //TODO remove
                /*
                for (; i < RowCount; i++)
                {
                    string ValueString = StringList[i];
                    if (ValueString.Length > 0)
                        ValueString += ", ";

                    ValueString += ColumnType.DefaultValue;
                    StringList[i] = ValueString;
                }*/
            }

            ValueStringList = StringList;
            return true;
        }
        #endregion
    }
}
