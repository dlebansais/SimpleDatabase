namespace Database.Internal
{
    internal interface IMySqlModifyOperation<TContext, TInternal> : IModifyOperation, IMySqlOperation<TContext, TInternal>
        where TContext : IContext
        where TInternal : IResultInternal
    {
    }
}
