namespace Database.Internal
{
    internal interface IMySqlQueryOperation<TContext, TInternal> : IQueryOperation<TContext, TInternal>, IMySqlOperation<TContext, TInternal>
        where TContext : IContext
        where TInternal : IResultInternal
    {
    }
}
