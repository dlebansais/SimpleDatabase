using Database;
using Database.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    public class TestSet
    {
        #region Setup
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            Assembly SimpleDatabaseAssembly;

            try
            {
                SimpleDatabaseAssembly = Assembly.Load("SimpleDatabase");
            }
            catch
            {
                SimpleDatabaseAssembly = null;
            }
            Assume.That(SimpleDatabaseAssembly != null);

            if (File.Exists("passwords.txt"))
            {
                using (FileStream fs = new FileStream("passwords.txt", FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        RootId = sr.ReadLine();
                        RootPassword = sr.ReadLine();
                    }
                }
            }
            else
            {
                RootId = "root";
                RootPassword = "";
            }

            try
            {
                TestSchema TestSchema = new TestSchema(false);
                ISimpleDatabase Database = new SimpleDatabase();
                ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
                Database.DeleteTables(Credential);
                Database.DeleteCredential(RootId, RootPassword, Credential);
            }
            catch
            {
            }
        }

        private static string RootId;
        private static string RootPassword;
        private static string Server = "localhost";
        private static string UserId = "test";
        private static string UserPassword = "test";
        #endregion

        #region Init
        [Test]
        public static void TestInitCredential()
        {
            TestSchema TestSchema = new TestSchema(false);
            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Init Credential 0");
            Assert.That(Credential.Server == Server, "Init Credential 1");
            Assert.That(Credential.UserId == UserId, "Init Credential 2");
            Assert.That(Credential.Password == UserPassword, "Init Credential 3");
            Assert.That(Credential.Schema == TestSchema, "Init Credential 4");
        }

        [Test]
        public static void TestInitDatabase()
        {
            TestSchema TestSchema = new TestSchema(false);
            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Init Database 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Init Database 1");

            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            Database.Initialize(ConnectorType, ConnectionOption);
            Assert.That(Database.ConnectorType == ConnectorType, "Init Database 2");
            Assert.That(Database.ConnectionOption == ConnectionOption, "Init Database 3");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestVerifyCredential(int isAsync)
        {
            TestSchema TestSchema = new TestSchema(false);
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;
            bool Success;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Verify Credential 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Verify Credential 1");

            Database.Initialize(ConnectorType, ConnectionOption);

            Assert.That(!Database.IsCredentialValid(Credential), "Verify Credential 2");
            Success = isAsync == 0 ? Database.CreateCredential(RootId, RootPassword, Credential) : await Database.CreateCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);
            Assert.That(Success, "Verify Credential 3");

            Success = isAsync == 0 ? Database.IsCredentialValid(Credential) : await Database.IsCredentialValidAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Verify Credential  4");

            if (isAsync == 0)
                Database.DeleteCredential(RootId, RootPassword, Credential);
            else
                await Database.DeleteCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);

            Success = isAsync == 0 ? Database.IsCredentialValid(Credential) : await Database.IsCredentialValidAsync(Credential);
            Wait(isAsync);
            Assert.That(!Success, "Verify Credential 5");

            Thread.Sleep(1000);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestCreateTables(int isAsync)
        {
            TestSchema TestSchema = new TestSchema(false);
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;
            bool Success;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Create Tables 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Create Tables 1");

            Database.Initialize(ConnectorType, ConnectionOption);
            Success = isAsync == 0 ? Database.CreateCredential(RootId, RootPassword, Credential) : await Database.CreateCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);
            Assert.That(Success, "Create Tables 2");
            Success = isAsync == 0 ? Database.CreateTables(Credential) : await Database.CreateTablesAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Create Tables 3");

            if (isAsync == 0)
                Database.DeleteTables(Credential);
            else
                await Database.DeleteTablesAsync(Credential);
            Wait(isAsync);

            if (isAsync == 0)
                Database.DeleteCredential(RootId, RootPassword, Credential);
            else
                await Database.DeleteCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);

            Success = isAsync == 0 ? Database.IsCredentialValid(Credential) : await Database.IsCredentialValidAsync(Credential);
            Wait(isAsync);
            Assert.That(!Success, "Create Tables 7");

            Thread.Sleep(1000);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestOpen(int isAsync)
        {
            TestSchema TestSchema = new TestSchema(false);
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;
            bool Success;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Open 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Open 1");

            Database.Initialize(ConnectorType, ConnectionOption);
            Success = isAsync == 0 ? Database.CreateCredential(RootId, RootPassword, Credential) : await Database.CreateCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);
            Assert.That(Success, "Open 2");
            Success = isAsync == 0 ? Database.CreateTables(Credential) : await Database.CreateTablesAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Open 3");

            Assert.That(!Database.IsOpen, "Open 4");
            Success = isAsync == 0 ? Database.Open(Credential) : await Database.OpenAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Open 5");
            Assert.That(Database.IsOpen, "Open 6");

            if (isAsync == 0)
                Database.Close();
            else
                await Database.CloseAsync();
            Wait(isAsync);

            Assert.That(!Database.IsOpen, "Open 7");

            if (isAsync == 0)
                Database.DeleteTables(Credential);
            else
                await Database.DeleteTablesAsync(Credential);
            Wait(isAsync);

            if (isAsync == 0)
                Database.DeleteCredential(RootId, RootPassword, Credential);
            else
                await Database.DeleteCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);

            Success = isAsync == 0 ? Database.IsCredentialValid(Credential) : await Database.IsCredentialValidAsync(Credential);
            Wait(isAsync);
            Assert.That(!Success, "Open 8");

            Thread.Sleep(1000);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestDeleteNonEmpty(int isAsync)
        {
            TestSchema TestSchema = new TestSchema(false);
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;
            bool Success;

            ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
            Assert.That(Credential != null, "Delete Non Empty 0");

            ISimpleDatabase Database = new SimpleDatabase();
            Assert.That(Database != null, "Delete Non Empty 1");

            Database.Initialize(ConnectorType, ConnectionOption);
            Success = isAsync == 0 ? Database.CreateCredential(RootId, RootPassword, Credential) : await Database.CreateCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);
            Assert.That(Success, "Delete Non Empty 2");
            Success = isAsync == 0 ? Database.CreateTables(Credential) : await Database.CreateTablesAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Delete Non Empty 3");
            Success = isAsync == 0 ? Database.Open(Credential) : await Database.OpenAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Delete Non Empty 4");

            IInsertResult InsertResult;
            IDeleteResult DeleteResult;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<ColumnValuePair<Guid>>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<ColumnValuePair<Guid>>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty) }));
            Wait(isAsync);

            Assert.That(InsertResult.Success, $"Delete Non Empty 5 ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                Database.Close();
            else
                await Database.CloseAsync();
            Wait(isAsync);

            if (isAsync == 0)
                Database.DeleteTables(Credential);
            else
                await Database.DeleteTablesAsync(Credential);
            Wait(isAsync);

            if (isAsync == 0)
                Database.DeleteCredential(RootId, RootPassword, Credential);
            else
                await Database.DeleteCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);

            Success = isAsync == 0 ? Database.IsCredentialValid(Credential) : await Database.IsCredentialValidAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Delete Non Empty 6");

            Success = isAsync == 0 ? Database.Open(Credential) : await Database.OpenAsync(Credential);
            Wait(isAsync);
            Assert.That(Success, "Delete Non Empty 7");

            if (isAsync == 0)
                DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty), 2));
            else
                DeleteResult = await Database.RunAsync(new DeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty), 2));
            Wait(isAsync);
            Assert.That(!DeleteResult.Success, "Delete Non Empty 8 (must fail)");

            if (isAsync == 0)
                DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty), 0));
            else
                DeleteResult = await Database.RunAsync(new DeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty), 0));
            Wait(isAsync);
            Assert.That(DeleteResult.Success, $"Delete Non Empty 8 ({DeleteResult.ErrorCode})");

            if (isAsync == 0)
                Database.Close();
            else
                await Database.CloseAsync();
            Wait(isAsync);

            if (isAsync == 0)
                Database.DeleteTables(Credential);
            else
                await Database.DeleteTablesAsync(Credential);
            Wait(isAsync);

            if (isAsync == 0)
                Database.DeleteCredential(RootId, RootPassword, Credential);
            else
                await Database.DeleteCredentialAsync(RootId, RootPassword, Credential);
            Wait(isAsync);

            Success = isAsync == 0 ? Database.IsCredentialValid(Credential) : await Database.IsCredentialValidAsync(Credential);
            Wait(isAsync);
            Assert.That(!Success, "Delete Non Empty 9");

            Thread.Sleep(1000);
        }
        #endregion

        #region Tools
        private static Guid guidKey0 = new Guid("{1BA0D7E9-039F-44E6-A966-CC67AC01A65D}");
        private static Guid guidKey1 = new Guid("{2FA55A73-0311-4818-8B34-1492308ADBF1}");
        private static Guid guidKey2 = new Guid("{16DC914E-CDED-41DD-AE23-43B62676159D}");

        private static void Wait(int isAsync)
        {
            if (isAsync != 0)
                Thread.Sleep(1000);
        }

        private static void InstallDatabase(string testName, bool dateTimeAsTick, out ICredential credential, out ISimpleDatabase database, out TestSchema testSchema)
        {
            testSchema = new TestSchema(dateTimeAsTick);
            ConnectorType ConnectorType = ConnectorType.MySql;
            ConnectionOption ConnectionOption = ConnectionOption.KeepAlive;

            credential = new Credential(Server, UserId, UserPassword, testSchema);
            Assert.That(credential != null, $"{testName} - Create Credential Object");

            database = new SimpleDatabase();
            Assert.That(database != null, $"{testName} - Create Database Object");

            database.Initialize(ConnectorType, ConnectionOption);
            Assert.That(!database.IsCredentialValid(credential), $"{testName} - Verify Credential Invalid");
            Assert.That(database.CreateCredential(RootId, RootPassword, credential), $"{testName} - Create Credential");
            Assert.That(database.IsCredentialValid(credential), $"{testName} - Verify Credential Valid");
            Assert.That(database.CreateTables(credential), $"{testName} - Create Tables");
            Assert.That(database.Open(credential), $"{testName} - Open");
        }

        private static void UninstallDatabase(string testName, ref ICredential credential, ref ISimpleDatabase database, ref TestSchema testSchema)
        {
            IDeleteResult DeleteResult;
            DeleteResult = database.Run(new DeleteContext(testSchema.Test0, 0));
            DeleteResult = database.Run(new DeleteContext(testSchema.Test1, 0));
            DeleteResult = database.Run(new DeleteContext(testSchema.Test2, 0));

            database.Close();
            database.DeleteTables(credential);
            database.DeleteCredential(RootId, RootPassword, credential);

            Thread.Sleep(5000);

            Assert.That(!database.IsCredentialValid(credential), $"{testName} - Verify Credential Invalid (after close). {database}");

            credential = null;
            database = null;
            testSchema = null;

            Thread.Sleep(1000);
        }
        #endregion

        #region Queries
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestSingleInsert(int isAsync)
        {
            string TestName = "Single Insert";

            TestSchema TestSchema;
            InstallDatabase(TestName, false, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            IJoinQueryResult JoinSelectResult;
            List<IResultRow> RowList;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            Wait(isAsync);

            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first key ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            Wait(isAsync);
            Assert.That(!InsertResult.Success, $"{TestName} - 0: Insert with no key (must fail)");

            ((SimpleDatabase)Database).IgnoreErrorCode = 1062;
            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey0) }));
            Wait(isAsync);
            Assert.That(!InsertResult.Success, $"{TestName} - 0: Insert same key (must fail)");

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 1) }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert new key and int ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test0.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test0.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 0: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 0: Read table result");

            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 2, $"{TestName} - 0: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey0, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey1, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1) && Test0_Row_1_1 == 1, $"{TestName} - 0: Check row 1, column 1");

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 0") }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 0") }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 1: Insert first row ({InsertResult.ErrorCode}) ... {InsertResult.Traces}");

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 1") }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<string>(TestSchema.Test1_String, "row 1") }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 1: Insert second row ({InsertResult.ErrorCode})");

            ((SimpleDatabase)Database).IgnoreErrorCode = 1062;
            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test1_Int, 1), new ColumnValuePair<string>(TestSchema.Test1_String, "row 2") }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test1_Int, 1), new ColumnValuePair<string>(TestSchema.Test1_String, "row 2") }));
            Wait(isAsync);
            Assert.That(!InsertResult.Success, $"{TestName} - 1: Insert with key (must fail)");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test1.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test1.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 1: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 1: Read table result");

            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 2, $"{TestName} - 1: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[0], out int Test1_Row_0_0) && Test1_Row_0_0 == 1, $"{TestName} - 1: Check row 0, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[0], out string Test1_Row_0_1) && Test1_Row_0_1 == "row 0", $"{TestName} - 1: Check row 0, column 1");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[1], out int Test1_Row_1_0) && Test1_Row_1_0 == 2, $"{TestName} - 1: Check row 1, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[1], out string Test1_Row_1_1) && Test1_Row_1_1 == "row 1", $"{TestName} - 1: Check row 1, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestJoinInsert(int isAsync)
        {
            string TestName = "Join Insert";

            TestSchema TestSchema;
            InstallDatabase(TestName, false, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            IJoinQueryResult JoinSelectResult;
            List<IResultRow> RowList;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test0.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test0.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 0: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 0: Read table result");

            // Columns are reordered by Guid
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 3, $"{TestName} - 0: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey2, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey0, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1), $"{TestName} - 0: Check row 1, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[2], out Guid Test0_Row_2_0) && Test0_Row_2_0 == guidKey1, $"{TestName} - 0: Check row 2, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[2], out int Test0_Row_2_1), $"{TestName} - 0: Check row 2, column 1");

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<string>(TestSchema.Test1_String, new List<string>() { "row 0", "row 1", "row 2" }) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<string>(TestSchema.Test1_String, new List<string>() { "row 0", "row 1", "row 2" }) }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 1: Insert first row ({InsertResult.ErrorCode} ... {InsertResult.Traces})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test1.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test1.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 1: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 1: Read table result");

            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 3, $"{TestName} - 1: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[0], out int Test1_Row_0_0) && Test1_Row_0_0 == 1, $"{TestName} - 1: Check row 0, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[0], out string Test1_Row_0_1) && Test1_Row_0_1 == "row 0", $"{TestName} - 1: Check row 0, column 1");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[1], out int Test1_Row_1_0) && Test1_Row_1_0 == 2, $"{TestName} - 1: Check row 1, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[1], out string Test1_Row_1_1) && Test1_Row_1_1 == "row 1", $"{TestName} - 1: Check row 1, column 1");
            Assert.That(TestSchema.Test1_Int.TryParseRow(RowList[2], out int Test1_Row_2_0) && Test1_Row_2_0 == 3, $"{TestName} - 2: Check row 2, column 0");
            Assert.That(TestSchema.Test1_String.TryParseRow(RowList[2], out string Test1_Row_2_1) && Test1_Row_2_1 == "row 2", $"{TestName} - 2: Check row 2, column 2");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestUpdate(int isAsync)
        {
            string TestName = "Update";

            TestSchema TestSchema;
            InstallDatabase(TestName, false, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            IUpdateResult UpdateResult;
            IJoinQueryResult JoinSelectResult;
            List<IResultRow> RowList;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }));
            else
                UpdateResult = await Database.RunAsync(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }));
            Wait(isAsync);
            Assert.That(UpdateResult.Success, $"{TestName} - 0: Update third keys ({UpdateResult.ErrorCode})");

            if (isAsync == 0)
                UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 20) }));
            else
                UpdateResult = await Database.RunAsync(new UpdateContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 20) }));
            Wait(isAsync);
            Assert.That(UpdateResult.Success, $"{TestName} - 0: Update second and third keys ({UpdateResult.ErrorCode})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test0.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test0.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 0: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 0: Read table result");

            // Columns are reordered by Guid
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 3, $"{TestName} - 0: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey2, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey0, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1), $"{TestName} - 0: Check row 1, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[2], out Guid Test0_Row_2_0) && Test0_Row_2_0 == guidKey1, $"{TestName} - 0: Check row 2, column 0");
            Assert.That(TestSchema.Test0_Int.TryParseRow(RowList[2], out int Test0_Row_2_1) && Test0_Row_2_1 == 20, $"{TestName} - 0: Check row 2, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestSingleDelete(int isAsync)
        {
            string TestName = "Single Delete";

            TestSchema TestSchema;
            InstallDatabase(TestName, false, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            IDeleteResult DeleteResult;
            IJoinQueryResult JoinSelectResult;
            List<IResultRow> RowList;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }, 0));
            else
                DeleteResult = await Database.RunAsync(new DeleteContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }, 0));
            Wait(isAsync);
            Assert.That(DeleteResult.Success, $"{TestName} - 0: Delete first key ({DeleteResult.ErrorCode})");
            Assert.That(DeleteResult.DeletedRowCount == 0, $"{TestName} - 0: Delete first key (no row) ({DeleteResult.DeletedRowCount})");

            if (isAsync == 0)
                DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2) }, 0));
            else
                DeleteResult = await Database.RunAsync(new DeleteContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2) }, 0));
            Wait(isAsync);
            Assert.That(DeleteResult.Success, $"{TestName} - 0: Delete first key again ({DeleteResult.ErrorCode})");
            Assert.That(DeleteResult.DeletedRowCount == 1, $"{TestName} - 0: Delete first key (one row) ({DeleteResult.DeletedRowCount})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test0.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test0.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 0: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 0: Read table result");

            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 2, $"{TestName} - 0: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey0, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[1], out Guid Test0_Row_1_0) && Test0_Row_1_0 == guidKey1, $"{TestName} - 0: Check row 1, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[1], out int Test0_Row_1_1), $"{TestName} - 0: Check row 1, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestMultiDelete(int isAsync)
        {
            string TestName = "Multi Delete";

            TestSchema TestSchema;
            InstallDatabase(TestName, false, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            IDeleteResult DeleteResult;
            IJoinQueryResult JoinSelectResult;
            List<IResultRow> RowList;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                DeleteResult = Database.Run(new DeleteContext(TestSchema.Test0, new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1 }), 2));
            else
                DeleteResult = await Database.RunAsync(new DeleteContext(TestSchema.Test0, new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1 }), 2));
            Wait(isAsync);
            Assert.That(DeleteResult.Success, $"{TestName} - 0: Delete two keys ({DeleteResult.ErrorCode})");
            Assert.That(DeleteResult.DeletedRowCount == 2, $"{TestName} - 0: Delete two keys (row count) ({DeleteResult.DeletedRowCount})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test0.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test0.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 0: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 0: Read table result");

            // Columns are reordered by Guid
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 1, $"{TestName} - 0: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test0_Guid.TryParseRow(RowList[0], out Guid Test0_Row_0_0) && Test0_Row_0_0 == guidKey2, $"{TestName} - 0: Check row 0, column 0");
            Assert.That(!TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1), $"{TestName} - 0: Check row 0, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestSingleQuery(int isAsync)
        {
            string TestName = "Single Query";

            TestSchema TestSchema;
            InstallDatabase(TestName, false, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            ISingleQueryResult SelectResult;
            List<IResultRow> RowList;
            IUpdateResult UpdateResult;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }) }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test0));
            else
                SelectResult = await Database.RunAsync(new SingleQueryContext(TestSchema.Test0));
            Wait(isAsync);
            Assert.That(SelectResult.Success, $"{TestName} - 0: Read table ({SelectResult.ErrorCode})");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 0: Read table result");
            Assert.That(SelectResult.RowList.Count == 3, $"{TestName} - 0: Read table result count ({SelectResult.RowList.Count})");
            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList[2].HasColumn(TestSchema.Test0_Guid), $"{TestName} - 0: Read table last row, guid");
            Assert.That(!RowList[2].HasColumn(TestSchema.Test0_Int), $"{TestName} - 0: Read table last row, int (must fail)");

            if (isAsync == 0)
            {
                UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 2) }));
                UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 3) }));
            }
            else
            {
                UpdateResult = await Database.RunAsync(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 2) }));
                Wait(isAsync);
                UpdateResult = await Database.RunAsync(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 3) }));
                Wait(isAsync);
            }

            if (isAsync == 0)
                SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test0, new List<IColumnDescriptor>() { TestSchema.Test0_Guid, TestSchema.Test0_Int }));
            else
                SelectResult = await Database.RunAsync(new SingleQueryContext(TestSchema.Test0, new List<IColumnDescriptor>() { TestSchema.Test0_Guid, TestSchema.Test0_Int }));
            Wait(isAsync);
            Assert.That(SelectResult.Success, $"{TestName} - 1: Read table ({SelectResult.ErrorCode})");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 1: Read table result");
            Assert.That(SelectResult.RowList.Count == 3, $"{TestName} - 1: Read table result count ({SelectResult.RowList.Count})");
            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList[1].HasColumn(TestSchema.Test0_Guid), $"{TestName} - 1: Read table middle row, guid");
            Assert.That(!RowList[1].HasColumn(TestSchema.Test0_Int), $"{TestName} - 1: Read table middle row, int");

            if (isAsync == 0)
                SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test0, new ColumnValueCollectionPair<int>(TestSchema.Test0_Int, new List<int>() { 3 }), new List<IColumnDescriptor>() { TestSchema.Test0_Guid, TestSchema.Test0_Int }));
            else
                SelectResult = await Database.RunAsync(new SingleQueryContext(TestSchema.Test0, new ColumnValueCollectionPair<int>(TestSchema.Test0_Int, new List<int>() { 3 }), new List<IColumnDescriptor>() { TestSchema.Test0_Guid, TestSchema.Test0_Int }));
            Wait(isAsync);
            Assert.That(SelectResult.Success, $"{TestName} - 1: Read table ({SelectResult.ErrorCode})");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 1: Read table result");
            Assert.That(SelectResult.RowList.Count == 1, $"{TestName} - 1: Read table result count ({SelectResult.RowList.Count})");
            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(TestSchema.Test0_Int.TryParseRow(RowList[0], out int Test0_Row_0_1) && Test0_Row_0_1 == 3, $"{TestName} - 0: Check row 0, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static async Task TestJoinQuery(int isAsync)
        {
            string TestName = "Join Query";

            TestSchema TestSchema;
            InstallDatabase(TestName, false, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            IJoinQueryResult JoinSelectResult;
            List<IResultRow> RowList;
            IUpdateResult UpdateResult;

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }) }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test0.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test0.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 0: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 0: Read table result");
            Assert.That(JoinSelectResult.RowList.Count == 3, $"{TestName} - 0: Read table result count ({JoinSelectResult.RowList.Count})");
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList[2].HasColumn(TestSchema.Test0_Guid), $"{TestName} - 0: Read table last row, guid");
            Assert.That(!RowList[2].HasColumn(TestSchema.Test0_Int), $"{TestName} - 0: Read table last row, int (must fail)");

            if (isAsync == 0)
                InsertResult = Database.Run(new InsertContext(TestSchema.Test1, 4, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<string>(TestSchema.Test1_String, new List<string>() { "row 0", "row 1", "row 2", "row 3" }) }));
            else
                InsertResult = await Database.RunAsync(new InsertContext(TestSchema.Test1, 4, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<string>(TestSchema.Test1_String, new List<string>() { "row 0", "row 1", "row 2", "row 3" }) }));
            Wait(isAsync);
            Assert.That(InsertResult.Success, $"{TestName} - 1: Insert first row ({InsertResult.ErrorCode})");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(TestSchema.Test1.All));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(TestSchema.Test1.All));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 1: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 1: Read table result");
            Assert.That(JoinSelectResult.RowList.Count == 4, $"{TestName} - 1: Read table result count ({JoinSelectResult.RowList.Count})");
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList[3].HasColumn(TestSchema.Test1_Int), $"{TestName} - 1: Read table last row, int");
            Assert.That(RowList[3].HasColumn(TestSchema.Test1_String), $"{TestName} - 1: Read table last row, string");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(new List<IColumnDescriptor>() { TestSchema.Test0_Guid }));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(new List<IColumnDescriptor>() { TestSchema.Test0_Guid }));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 0: Read table, one column ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 0: Read table result, one column");
            Assert.That(JoinSelectResult.RowList.Count == 3, $"{TestName} - 0: Read table result count, one column ({JoinSelectResult.RowList.Count})");
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList[2].HasColumn(TestSchema.Test0_Guid), $"{TestName} - 0: Read table last row, guid");
            Assert.That(!RowList[2].HasColumn(TestSchema.Test0_Int), $"{TestName} - 0: Read table last row, int (must fail)");

            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(new List<IColumnDescriptor>() { TestSchema.Test1_String }));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(new List<IColumnDescriptor>() { TestSchema.Test1_String }));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - 1: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - 1: Read table result");
            Assert.That(JoinSelectResult.RowList.Count == 4, $"{TestName} - 1: Read table result count ({JoinSelectResult.RowList.Count})");
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(!RowList[3].HasColumn(TestSchema.Test1_Int) || !(((SimpleDatabase)Database).CanIntBeNULL), $"{TestName} - 1: Read table last row, int (must fail)");
            Assert.That(RowList[3].HasColumn(TestSchema.Test1_String), $"{TestName} - 1: Read table last row, string");

            if (isAsync == 0)
            {
                UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 2) }));
                UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 3) }));
            }
            else
            {
                UpdateResult = await Database.RunAsync(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey1), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 2) }));
                Wait(isAsync);
                UpdateResult = await Database.RunAsync(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 3) }));
                Wait(isAsync);
            }

            IJoin Join = new LeftJoin(TestSchema.Test1_Int, TestSchema.Test0_Int);
            if (isAsync == 0)
                JoinSelectResult = Database.Run(new JoinQueryContext(Join, new List<IColumnDescriptor>() { TestSchema.Test1_String, TestSchema.Test0_Guid }));
            else
                JoinSelectResult = await Database.RunAsync(new JoinQueryContext(Join, new List<IColumnDescriptor>() { TestSchema.Test1_String, TestSchema.Test0_Guid }));
            Wait(isAsync);
            Assert.That(JoinSelectResult.Success, $"{TestName} - Join: Read table ({JoinSelectResult.ErrorCode})");
            Assert.That(JoinSelectResult.RowList != null, $"{TestName} - Join: Read table result");
            RowList = new List<IResultRow>(JoinSelectResult.RowList);
            Assert.That(RowList.Count == 3, $"{TestName} - Join: Read table result count ({RowList.Count})");
            Assert.That(RowList[0].HasColumn(TestSchema.Test0_Guid), $"{TestName} - Join: Read table row 0 column 0");
            Assert.That(RowList[0].HasColumn(TestSchema.Test1_String), $"{TestName} - Join: Read table row 0 column 1");
            Assert.That(RowList[1].HasColumn(TestSchema.Test0_Guid), $"{TestName} - Join: Read table row 1 column 0");
            //Assert.That(!RowList[1].HasColumn(TestSchema.Test1_String) || !(((SimpleDatabase)Database).CanIntBeNULL), $"{TestName} - Join: Read table row 1 column 1 (must fail)");
            Assert.That(RowList[2].HasColumn(TestSchema.Test0_Guid), $"{TestName} - Join: Read table row 2 column 0");
            //Assert.That(RowList[2].HasColumn(TestSchema.Test1_String), $"{TestName} - Join: Read table row 2 column 1");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }
        #endregion

        #region DateTime
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public static void TestDateTime(int n)
        {
            string TestName = (n == 0 ? "DATETIME" : "BIGINT");

            TestSchema TestSchema;
            InstallDatabase(TestName, n != 0, out ICredential Credential, out ISimpleDatabase Database, out TestSchema);

            IInsertResult InsertResult;
            IUpdateResult UpdateResult;
            ISingleQueryResult SelectResult;
            List<IResultRow> RowList;

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
            Assert.That(InsertResult.Success, $"{TestName} - 0: Insert first 3 keys ({InsertResult.ErrorCode})");

            UpdateResult = Database.Run(new UpdateContext(TestSchema.Test2, new ColumnValuePair<DateTime>(TestSchema.Test2_DateTime, Dates0[2]), new List<IColumnValuePair>() { new ColumnValuePair<DateTime>(TestSchema.Test2_DateTime, Dates1[0]) }));
            Assert.That(UpdateResult.Success, $"{TestName} - 0: Update third keys ({UpdateResult.ErrorCode})");

            SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test2, TestSchema.Test2.All));
            Assert.That(SelectResult.Success, $"{TestName} - 0: Read table ({SelectResult.ErrorCode})");
            Assert.That(SelectResult.RowList != null, $"{TestName} - 0: Read table result");

            RowList = new List<IResultRow>(SelectResult.RowList);
            Assert.That(RowList != null && RowList.Count == 3, $"{TestName} - 0: Count rows ({RowList.Count})");
            Assert.That(TestSchema.Test2_DateTime.TryParseRow(RowList[0], out DateTime Test0_Row_0_1) && Test0_Row_0_1 == Dates0[0], $"{TestName} - 0: Check row 0, column 1");
            if (TestSchema.DateTimeAsTicks)
                Assert.That(TestSchema.Test2_DateTime.TryParseRow(RowList[1], out DateTime Test0_Row_1_1) && Test0_Row_1_1 == Dates0[1], $"{TestName} - 0: Check row 1, column 1");
            Assert.That(TestSchema.Test2_DateTime.TryParseRow(RowList[2], out DateTime Test0_Row_2_1) && Test0_Row_2_1 == Dates1[0], $"{TestName} - 0: Check row 2, column 1");

            UninstallDatabase(TestName, ref Credential, ref Database, ref TestSchema);
        }
        #endregion
    }
}
