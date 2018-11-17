using MySql.Data.MySqlClient;
using System;

namespace Database.Internal
{
    internal interface IMySqlActiveOperation : IActiveOperation
    {
        Func<MySqlCommand, IResultInternal, string> MySqlFinalizerBase { get; }
    }

    internal interface IMySqlActiveOperation<TInternal> : IActiveOperation<TInternal>, IMySqlActiveOperation
        where TInternal : IResultInternal
    {
        Func<MySqlCommand, TInternal, string> MySqlFinalizer { get; }
    }

    internal class MySqlActiveOperation<TInternal> : ActiveOperation<TInternal>, IMySqlActiveOperation<TInternal>
        where TInternal : IResultInternal
    {
        public MySqlActiveOperation(TInternal result, Func<MySqlCommand, TInternal, string> finalizer)
            : base(result)
        {
            MySqlFinalizer = finalizer;
        }

        public MySqlActiveOperation(TInternal result)
            : base(result)
        {
        }

        public Func<MySqlCommand, TInternal, string> MySqlFinalizer { get; }
        Func<MySqlCommand, IResultInternal, string> IMySqlActiveOperation.MySqlFinalizerBase { get { return (MySqlCommand command, IResultInternal result) => MySqlFinalizer(command, (TInternal)result); } }
    }
}
