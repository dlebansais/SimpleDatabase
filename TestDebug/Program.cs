using Database;
using Database.Types;
using System;
using System.Collections.Generic;
using Test;

namespace TestDebug
{
    class Program
    {
        private static Guid guidKey0 = new Guid("{1BA0D7E9-039F-44E6-A966-CC67AC01A65D}");
        private static Guid guidKey1 = new Guid("{2FA55A73-0311-4818-8B34-1492308ADBF1}");
        private static Guid guidKey2 = new Guid("{16DC914E-CDED-41DD-AE23-43B62676159D}");

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

            IDeleteResult DeleteResult;
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test1, 0));

            IInsertResult InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<ColumnValuePair<Guid>>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty) }));

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);

            Database.Open(Credential);
            IDeleteResult SingleDeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty), 0));

            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test1, 0));

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
        }
    }
}
