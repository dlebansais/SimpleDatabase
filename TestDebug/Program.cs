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

            IMultiRowDeleteResult DeleteResult;
            DeleteResult = Database.Run(new MultiRowDeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new MultiRowDeleteContext(TestSchema.Test1, 0));

            ISingleInsertResult InsertResult;

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 0") }));

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 1") }));

            InsertResult = Database.Run(new SingleInsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test1_Int, 1), new ColumnValuePair<string>(TestSchema.Test1_String, "row 0") }));

            IMultiQueryResult SelectResult;
            SelectResult = Database.Run(new MultiQueryContext(TestSchema.Test0.All));
            List<IResultRow> RowList = new List<IResultRow>(SelectResult.RowList);
            TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0);
            TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1);
            TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0);
            TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1);


            DeleteResult = Database.Run(new MultiRowDeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new MultiRowDeleteContext(TestSchema.Test1, 0));

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
        }
    }
}
