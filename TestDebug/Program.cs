using Database;

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
            ICredential Credential = new Credential("localhost", "test@localhost", "root", TestSchema);

            Database.IsCredentialValid(Credential);
            Database.CreateCredential(RootId, RootPassword, Credential);
        }
    }
}
