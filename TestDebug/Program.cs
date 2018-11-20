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
            List<IResultRow> RowList;
            IInsertResult InsertResult;
            IDeleteResult DeleteResult;
            IJoinQueryResult QueryResult;
            IUpdateResult UpdateResult;
            ISingleQueryResult SelectResult;

            Success = Database.IsCredentialValid(Credential);
            Success = Database.CreateCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
            Success = Database.CreateTables(Credential);
            Success = Database.Open(Credential);

            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test1, 0));

            InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }) }));
            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 2) }));
            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 3) }));

            SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test0, new List<IColumnDescriptor>() { TestSchema.Test0_Guid, TestSchema.Test0_Int }));
            RowList = new List<IResultRow>(SelectResult.RowList);

            SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test0, new ColumnValueCollectionPair<int>(TestSchema.Test0_Int, new List<int>() { 3 }), new List<IColumnDescriptor>() { TestSchema.Test0_Guid, TestSchema.Test0_Int }));
            RowList = new List<IResultRow>(SelectResult.RowList);
            TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1);



            InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }) }));
            InsertResult = Database.Run(new InsertContext(TestSchema.Test1, 4, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<string>(TestSchema.Test1_String, new List<string>() { "row 0", "row 1", "row 2", "row 3" }) }));
            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 2) }));
            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 3) }));

            Dictionary<IColumnDescriptor, IColumnDescriptor> Join = new Dictionary<IColumnDescriptor, IColumnDescriptor>()
            {
                { TestSchema.Test1_Int, TestSchema.Test0_Int },
            };
            QueryResult = Database.Run(new JoinQueryContext(Join, new List<IColumnDescriptor>() { TestSchema.Test1_String, TestSchema.Test0_Guid }));


            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, 0));
            DeleteResult = Database.Run(new DeleteContext(TestSchema.Test1, 0));

            Database.Close();
            Database.DeleteTables(Credential);
            Database.DeleteCredential(RootId, RootPassword, Credential);
            Success = Database.IsCredentialValid(Credential);
        }
    }
}
