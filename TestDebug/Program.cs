using Database;
using System;
using System.Collections.Generic;

namespace TestDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                return;

            string RootId = args[0];
            string RootPassword = args[1];

            ISimpleDatabase Database = new SimpleDatabase();
            Database.Initialize(ConnectorType.MySql, ConnectionOption.KeepAlive);

            ITestSchema TestSchema = new TestSchema();
            ICredential Credential = new Credential("localhost", "test", "test", TestSchema);

            bool Success;

            Success = Database.IsCredentialValid(Credential);
            Success = Database.CreateCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
            Success = Database.CreateTables(Credential);
            Success = Database.Open(Credential);

            ISingleInsertResult InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<ColumnValuePair<Guid>>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty) }));
            Success = InsertResult.Success;

            ISingleRowDeleteResult DeleteResult = Database.Run(new SingleRowDeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty)));
            Success = DeleteResult.Success;

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
        }
    }
}
