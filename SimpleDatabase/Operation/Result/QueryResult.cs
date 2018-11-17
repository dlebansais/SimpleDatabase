using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Database
{
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
}

namespace Database.Internal
{
    internal interface IQueryResultInternal : IResultInternal, IQueryResult
    {
    }

    internal abstract class QueryResult : Result, IQueryResultInternal
    {
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

        #region Properties
        public IReadOnlyCollection<IResultRow> RowList { get; private set; }
        #endregion

        #region Descendant Interface
        public void SetCompletedWithResult(bool success, List<IResultRow> Rows)
        {
            if (success)
            {
                List<IResultRow> Result = new List<IResultRow>();

                foreach (IResultRow Row in Rows)
                    Result.Add(Row);

                RowList = Result;
            }

            base.SetCompleted(success);
        }

        public override void SetCompleted(bool success)
        {
            Debug.Assert(false);
            base.SetCompleted(success);
        }
        #endregion
    }
}
