using Database;
using Database.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test;

namespace TestDebug
{
    class Program
    {
        private static Guid guidKey0 = new Guid("{1BA0D7E9-039F-44E6-A966-CC67AC01A65D}");
        private static Guid guidKey1 = new Guid("{2FA55A73-0311-4818-8B34-1492308ADBF1}");
        private static Guid guidKey2 = new Guid("{16DC914E-CDED-41DD-AE23-43B62676159D}");

        static async Task Main(string[] args)
        {
            if (args.Length < 2)
                return;

            string RootId = args[0];
            string RootPassword = args[1];

            ISimpleDatabase Database = new SimpleDatabase();
            Database.Initialize(ConnectorType.MySql, ConnectionOption.KeepAlive, true);
            bool IsServerStarted = Database.IsServerStarted;

            ITestSchema TestSchema = new TestSchema(false);
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
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test2, 0));

            await Test(Database, TestSchema);

            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test1, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test2, 0));

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);

            Thread.Sleep(5000);
            Success = Database.IsCredentialValid(Credential);
        }

        static async Task Test(ISimpleDatabase Database, ITestSchema TestSchema)
        {
            List<IResultRow> RowList;
            IInsertResult InsertResult;
            //IDeleteResult DeleteResult;
            IJoinQueryResult JoinQueryResult;
            //IUpdateResult UpdateResult;
            //ISingleQueryResult SelectResult;

            int isAsync = 1;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));

            Thread.Sleep(5000);

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));

            Thread.Sleep(5000);

            ((SimpleDatabase)Database).IgnoreErrorCode = 1062;
            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));

            Thread.Sleep(5000);

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));

            Thread.Sleep(5000);

            if (isAsync == 0)
                JoinQueryResult = Database.Run(new JoinQueryContext(TestSchema.Test0.All));
            else
                JoinQueryResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test0.All));

            RowList = new List<IResultRow>(JoinQueryResult.RowList);

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 0") }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 0") }));

            Thread.Sleep(5000);

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 1") }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 1") }));

            Thread.Sleep(5000);

            ((SimpleDatabase)Database).IgnoreErrorCode = 1062;
            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test1_Int, 1), new ColumnValuePair<string>(TestSchema.Test1_String, "row 2") }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test1_Int, 1), new ColumnValuePair<string>(TestSchema.Test1_String, "row 2") }));

            Thread.Sleep(5000);

            if (isAsync == 0)
                JoinQueryResult = Database.Run(new JoinQueryContext(TestSchema.Test1.All));
            else
                JoinQueryResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test1.All));

            RowList = new List<IResultRow>(JoinQueryResult.RowList);
        }
    }
}
