using Database;
using Database.Types;

namespace Test
{
    public interface ITestSchema : ISchemaDescriptor
    {
        ITableDescriptor Test0 { get; }
        IColumnDescriptorGuid Test0_Guid { get; }
        IColumnDescriptorInt Test0_Int { get; }

        ITableDescriptor Test1 { get; }
        IColumnDescriptorInt Test1_Int { get; }
        IColumnDescriptorString Test1_String { get; }

        ITableDescriptor Test2 { get; }
        IColumnDescriptorInt Test2_Int { get; }
        IColumnDescriptorDateTime Test2_DateTime { get; }
    }

    public class TestSchema : SchemaDescriptor, ITestSchema
    {
        public ITableDescriptor Test0 { get; }
        public IColumnDescriptorGuid Test0_Guid { get; }
        public IColumnDescriptorInt Test0_Int { get; }

        public ITableDescriptor Test1 { get; }
        public IColumnDescriptorInt Test1_Int { get; }
        public IColumnDescriptorString Test1_String { get; }

        public ITableDescriptor Test2 { get; }
        public IColumnDescriptorInt Test2_Int { get; }
        public IColumnDescriptorDateTime Test2_DateTime { get; }

        public TestSchema(bool dateTimeAsTick)
            : base("mytest", dateTimeAsTick)
        {
            Test0 = new TableDescriptor(this, "Test0");
            Test0_Guid = new ColumnDescriptorGuid(Test0, "column_guid");
            Test0_Int = new ColumnDescriptorInt(Test0, "column_int");

            Test1 = new TableDescriptor(this, "Test1");
            Test1_Int = new ColumnDescriptorInt(Test1, "column_int");
            Test1_String = new ColumnDescriptorString(Test1, "column_string");

            Test2 = new TableDescriptor(this, "Test2");
            Test2_Int = new ColumnDescriptorInt(Test2, "column_int");
            Test2_DateTime = new ColumnDescriptorDateTime(Test2, "column_datetime");
        }
    }
}
