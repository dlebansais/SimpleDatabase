using Database;
using Database.Types;

namespace TestDebug
{
    public interface ITestSchema : ISchemaDescriptor
    {
    }

    public class TestSchema : SchemaDescriptor, ITestSchema
    {
        public ITableDescriptor Test0;
        public IColumnDescriptorGuid Test0_Guid;

        public TestSchema()
            : base("test")
        {
            Test0 = new TableDescriptor(this, "Test0");
            Test0_Guid = new ColumnDescriptorGuid(Test0, "guid");
        }
    }
}
