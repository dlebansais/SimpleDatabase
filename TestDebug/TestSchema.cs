using Database;
using Database.Types;

namespace TestDebug
{
    public interface ITestSchema : ISchemaDescriptor
    {
        ITableDescriptor Test0 { get; }
        IColumnDescriptorGuid Test0_Guid { get; }
    }

    public class TestSchema : SchemaDescriptor, ITestSchema
    {
        public ITableDescriptor Test0 { get; }
        public IColumnDescriptorGuid Test0_Guid { get; }

        public TestSchema()
            : base("test")
        {
            Test0 = new TableDescriptor(this, "Test0");
            Test0_Guid = new ColumnDescriptorGuid(Test0, "guid");
        }
    }
}
