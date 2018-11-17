using MySql.Data.MySqlClient;

namespace Database.Internal
{
    internal interface IMySqlOperation : IOperation
    {
    }

    internal interface IMySqlOperation<TContext, TInternal> : IOperation<TContext, TInternal>, IMySqlOperation
        where TContext : IContext
        where TInternal : IResultInternal
    {
        string GetCommandText();
        string FinalizeOperation(MySqlCommand command, TInternal result);
    }
}
