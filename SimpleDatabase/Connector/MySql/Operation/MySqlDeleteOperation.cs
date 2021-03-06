﻿using Database.Types;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database.Internal
{
    internal interface IMySqlDeleteOperation : IDeleteOperation, IMySqlModifyOperation<IDeleteContext, IDeleteResultInternal>
    {
    }

    internal class MySqlDeleteOperation : DeleteOperation, IMySqlDeleteOperation
    {
        #region Init
        public MySqlDeleteOperation(IDeleteContext context)
            : base(context)
        {
        }
        #endregion

        #region Client Interface
        public virtual string GetCommandText()
        {
            string TableName = GetTableName();
            string ConstraintString = GetConstraintString();

            string Result;
            if (ConstraintString != null)
                Result = $"DELETE FROM {TableName} WHERE {ConstraintString};";
            else
                Result = $"DELETE FROM {TableName};";

            return Result;
        }

        public virtual string FinalizeOperation(MySqlCommand command, IDeleteResultInternal result)
        {
            try
            {
                int DeletedRowCount = command.EndExecuteNonQuery(result.AsyncResult);

                if (DeletedRowCount >= Context.ExpectedDeletedCount)
                {
                    result.SetCompletedWithCount(DeletedRowCount);
                    return $"succeeded, {DeletedRowCount} row(s) deleted";
                }
                else
                {
                    result.SetCompleted(false, ResultError.ErrorMissingDeletedRows);
                    return "failed";
                }
            }
            catch
            {
                result.SetCompleted(false, ResultError.ErrorExceptionCompletingDelete);
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

                ConstraintString += $"({ColumnName}={FormattedValue})";
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
                ConstraintString += $"({ColumnName}={FormattedValue})";
            }

            return ConstraintString;
        }

        protected virtual string GetConstraintString()
        {
            if (Context.MultipleConstraintList != null)
                return GetMultipleConstraintString();
            else if (Context.SingleConstraintEntry != null)
                return GetSingleConstraintString();
            else
                return null;
        }
        #endregion
    }
}
