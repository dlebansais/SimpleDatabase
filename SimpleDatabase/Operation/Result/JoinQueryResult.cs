﻿using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    ///     Represents the result of a request to query values from several tables.
    /// </summary>
    public interface IJoinQueryResult : IQueryResult
    {
    }
}

namespace Database.Internal
{
    internal interface IJoinQueryResultInternal : IQueryResultInternal, IJoinQueryResult
    {
    }

    internal class JoinQueryResult : QueryResult, IJoinQueryResultInternal
    {
        public JoinQueryResult(IJoinQueryOperation operation, IAsyncResult asyncResult)
            : base(operation, asyncResult)
        {
        }

        public JoinQueryResult(bool success)
            : base(success)
        {
        }
    }
}