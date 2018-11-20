using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents the result of a request that only query values in one or more tables, and does not modify them.
    /// </summary>
    public interface IQueryResult : IResult
    {
        /// <summary>
        ///     Gets the rows returned by the query.
        /// </summary>
        /// <returns>
        ///     The rows returned by the query.
        /// </returns>
        IReadOnlyCollection<IResultRow> RowList { get; }
    }
    #endregion
}

namespace Database.Internal
{
    internal interface IQueryResultInternal : IResultInternal, IQueryResult
    {
        void SetCompletedWithRows(List<IResultRow> Rows);
    }

    internal abstract class QueryResult : Result, IQueryResultInternal
    {
        #region Init
        public QueryResult(IQueryOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
            RowList = null;
        }

        public QueryResult(bool success)
            : base(success)
        {
            RowList = null;
        }
        #endregion

        #region Properties
        public IReadOnlyCollection<IResultRow> RowList { get; private set; }
        #endregion

        #region Descendant Interface
        public void SetCompletedWithRows(List<IResultRow> Rows)
        {
            List<IResultRow> Result = new List<IResultRow>();

            foreach (IResultRow Row in Rows)
                Result.Add(Row);

            RowList = Result;

            base.SetCompleted(true);
        }

        public override void SetCompleted(bool success)
        {
            Debug.Assert(!success);
            base.SetCompleted(false);
        }
        #endregion
    }
}
