using Database;
using Database.Types;
using System;
using System.Collections.Generic;
using System.Threading;
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

            ITestSchema TestSchema = new TestSchema(false);
            ICredential Credential = new Credential("localhost", "test", "test", TestSchema);

            bool Success;
            List<IResultRow> RowList;
            IInsertResult InsertResult;
            IDeleteResult DeleteResult;
            //IJoinQueryResult QueryResult;
            IUpdateResult UpdateResult;
            ISingleQueryResult SelectResult;

            Success = Database.IsCredentialValid(Credential);
            Success = Database.CreateCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
            Success = Database.CreateTables(Credential);
            Success = Database.Open(Credential);

            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test1, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test2, 0));




            List<DateTime> Dates0 = new List<DateTime>()
            {
                new DateTime(2000, 10, 15, 10, 9, 58, 244, DateTimeKind.Utc),
                DateTime.UtcNow,
                new DateTime(2012, 2, 22, 7, 54, 32, 687, DateTimeKind.Utc),
            };
            List<DateTime> Dates1 = new List<DateTime>()
            {
                new DateTime(2034, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            InsertResult = Database.Run(new InsertContext(TestSchema.Test2, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<DateTime>(TestSchema.Test2_DateTime, Dates0), }));

            Thread.Sleep(1000);
            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test2, new ColumnValuePair<DateTime>(TestSchema.Test2_DateTime, Dates0[2]), new List<IColumnValuePair>() { new ColumnValuePair<DateTime>(TestSchema.Test2_DateTime, Dates1[0]) }));

            SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test2, TestSchema.Test2.All));

            RowList = new List<IResultRow>(SelectResult.RowList);
            Success = (TestSchema.Test2_DateTime.TryParseRow(RowList[0], out DateTime Test0_Row_0_1) && Test0_Row_0_1 == Dates0[0]);
            if (TestSchema.DateTimeAsTicks)
                Success = (TestSchema.Test2_DateTime.TryParseRow(RowList[1], out DateTime Test0_Row_1_1) && Test0_Row_1_1 == Dates0[1]);
            Success = (TestSchema.Test2_DateTime.TryParseRow(RowList[2], out DateTime Test0_Row_2_1) && Test0_Row_2_1 == Dates1[0]);

            IJoin Join = new LeftJoin(TestSchema.Test1_Int, TestSchema.Test0_Int);
            Database.Run(new JoinQueryContext(Join, new List<IColumnDescriptor>() { TestSchema.Test1_String, TestSchema.Test0_Guid }));



            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test1, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test2, 0));

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
        }
    }
}
