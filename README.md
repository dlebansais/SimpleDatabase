# Status: beta
TODO:
- [ ] Testing.
- [ ] Synchronous execution of fast requests.
- [ ] Doc and example.

# Simple Database
A C# layer over MySql for basic operations. Strongly typed, async, nothrow.

![Build Status](https://img.shields.io/travis/dlebansais/SimpleDatabase/master.svg)

This class library provides access to databases seen as collections and dictionaries of schemas, tables, columns and rows. The object-oriented part appears when one adds custom data types.

The library is strongly typed in that, if a column in contains data of a given type, all C# code using the library manipulate variables of that type, and no `object` or `var` is involved.
Every operation other than setting up the environment has an xxAsync version that can be plugged to a user interface as any other async method.

When possible, methods of the library will not throw any exception. This doesn't apply to invalid arguments, although some combination of arguments can lead to invalid SQL text but may not be caught when the operation is initiated.

## Example

The code below provides a simple example of how to use the library. More complete examples are provided later, this example is just intended to help the reader check if this is what they are looking for.

  ```cs
  ISimpleDatabase Database = new SimpleDatabase();
  Database.Initialize(ConnectorType.MySql, ConnectionOption.KeepAlive);

  ITestSchema TestSchema = new TestSchema();
  ICredential Credential = new Credential("localhost", "test", "test", TestSchema);

  Database.Open(Credential);

  IInsertContext InsertContext =
    new InsertContext(TestSchema.mytest,
      new List<IColumnValuePair>()
        { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, myguid) })

  IInsertResult InsertResult = Database.Run(InsertContext);
  ```
    
The code above opens the database and insert a new row with guid value `myguid` in the *mytest* table.

# Reference

## Defining your database

A the core of the library is a database description class for your application. You define your own class, inheriting from `SchemaDescriptor`, and enumerate your tables and columns in the contructor. Below is a simple example, that defines two tables in a schema named *mytest*:

  ```cs
  public class TestSchema : SchemaDescriptor
  {
    public ITableDescriptor Test0 { get; }
    public IColumnDescriptorGuid Test0_Guid { get; }
    public IColumnDescriptorInt Test0_Int { get; }

    public ITableDescriptor Test1 { get; }
    public IColumnDescriptorInt Test1_Int { get; }
    public IColumnDescriptorString Test1_String { get; }

    public TestSchema()
        : base("mytest")
    {
      Test0 = new TableDescriptor(this, "Test0");
      Test0_Guid = new ColumnDescriptorGuid(Test0, "column_guid");
      Test0_Int = new ColumnDescriptorInt(Test0, "column_int");

      Test1 = new TableDescriptor(this, "Test1");
      Test1_Int = new ColumnDescriptorInt(Test1, "column_int");
      Test1_String = new ColumnDescriptorString(Test1, "column_string");
    }
  }
  ```

This will automatically fill and organize internal collections in `SchemaDescriptor`, and hence, in `TestSchema` as well.  
In the remaining of the document, we will refer to this schema for examples. 
 
## Setting up

The library contains several methods to install the database (though it assumes the server is started).
1. To create a new SQL user and a new SQL database, call the `CreateCredential` method.

  ```cs
  ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
  ISimpleDatabase Database = new SimpleDatabase();
  Database.Initialize(ConnectorType, ConnectionOption);
  Database.CreateCredential(RootId, RootPassword, Credential);
  ```

2. The database at this stage is empty (it has no table). Use the CreateTables method to remedy to this situation.

  ```cs
  Database.CreateTables(Credential);
  ```

## Accuracy of DateTime

The default format for C# `DateTime` values is the "DATETIME(6)" MySQL format. This format doesn't store time as accurately as `DateTime`, but is human-readable.

If accuracy is more relevant for you, set the optional argument `dateTimeAsTick` to true when calling the `SchemaDescriptor` constructor, and the "BIGINT" MySQL format will be used instead.

This setting applies to all tables of a schema. To overwrite it for a single column, specify the `dateTimeAsTick` argument when creating the column. 

## Opening the database

For every run of your application, you need to open a new session:

  ```cs
  ICredential Credential = new Credential(Server, UserId, UserPassword, TestSchema);
  ISimpleDatabase Database = new SimpleDatabase();
  Database.Initialize(ConnectorType, ConnectionOption);
  Database.Open(Credential);
  ```

## Perfoming operations

All operations (queries, update, deleting...) are performed as follow.
1. You create and fill a context object, for example `QueryContext` (see the [Selecting data](#selecting-data) section for a specific example).
2. You call the corresponding `Database.Run` method. There is one method per context type, all called `Run`.
3. Alternatively, you call `Database.RunAsync` for asynchronous execution.
4. When execution is completed, `Run` returns one of the `xxResult` objects, matching the context used. For instance, in the `QueryContext` case, it will return a `IQueryResult` object.
5. You can inspect the `Success` property of this object, and for query operations the `RowList` property.

When a value is associated to a column, either because you insert it or because it's reported in the row list, the class used is `IColumnValuePair` with the specific type of the column. For example, `IColumnValuePair<Guid>` if the column contains guids.

When several values are associated to the column, use `IColumnValueCollectionPair` in one of its type-specific incarnations (ex: `IColumnValueCollectionPair<Guid>`).

If you want to address all columns of a table, you can use the `All` property of `ITableDescriptor`.

The next sections describe each operation and their details.
  
## Inserting new data

The library allows you to insert data either one row at a time, or several rows together.

To insert a single row, create an `InsertContext` object, with arguments the list of columns for which you provide a value. The following example inserts a new row in the *Test0* table, providing the value for the *column_guid* column but not for other columns.

  ```cs
  IInsertResult InsertResult = Database.Run(new InsertContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, new Guid("{1BA0D7E9-039F-44E6-A966-CC67AC01A65D}")) }));
  ```

To insert more than one row, provide the number of rows you want to insert, and for each of them a `IColumnValueCollectionPair` that will list all values for that row. Note that all value lists must have exactly as many values as rows inserted. If you want to keep some rows without values, you will need a more complicated sequence, for example by inserting each row seperately.
The following example inserts three rows, with guids `guidKey0`, `guidKey1` and `guidKey2` (not explicited here, for clarity).

  ```cs
  IInsertResult InsertResult = Database.Run(new InsertContext(TestSchema.Test0, 3, new List<IColumnValueCollectionPair>() { new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1, guidKey2 }), }));
  ```

## Updating data

Updating is done one set of values at a time, with the `UpdateContext` object. One set of values means you can assign only one new value in each column, but in can be on several rows together.

Updating can either target all rows for which a column contains a given value (*WHERE column = 'xxx'*) or rows for which several columns have a specific value (*WHERE columnX = 'xxx' AND columnY = 'yyy' AND columnZ = 'zzz'*).

Here is an example of the former:

  ```cs
  IUpdateResult UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey), new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }));
  ```

This example replaces all values in *column_int*, in rows for which *column_guid* is equal to `guidKey`, with the int value `10`.

Here is now an example of updating rows where several columns must match a given value:

  ```cs
  UpdateResult = Database.Run(new UpdateContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey), new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }, new List<IColumnValuePair>() { new ColumnValuePair<int>(TestSchema.Test0_Int, 20) }));
  ```

This time, rows that are updated are those for which *column_guid* is equal to `guidKey` and *column_int* is equal to `10`.

## Deleting rows

Deleting rows has more options. When performing a delete operation, you can set a minimum number of rows to delete for the operation to be considered successful. This can be zero, but typically it can be the number of rows returned by a query targetting specific values. If you use the same targetting object for the delete, the number of rows returned, and the delete is successful, you know all rows have been deleted.

Deleting rows also has the following options: 

1. Delete all rows in a table
  ```cs
  Database.Run(new DeleteContext(TestSchema.Test0, 0));
  ```
2. Delete rows for which a column has a specific value
  ```cs
  Database.Run(new DeleteContext(TestSchema.Test0, new ColumnValuePair<Guid>(TestSchema.Test0_Guid, Guid.Empty), 1234));
  ```
3. Delete rows for which several columns each have a specific value
  ```cs
  Database.Run(new DeleteContext(TestSchema.Test0, new List<IColumnValuePair>() { new ColumnValuePair<Guid>(TestSchema.Test0_Guid, guidKey2), new ColumnValuePair<int>(TestSchema.Test0_Int, 10) }, 0));
  ```
3. Delete rows for which a column has a one value among many.
  ```cs
  Database.Run(new DeleteContext(TestSchema.Test0, new ColumnValueCollectionPair<Guid>(TestSchema.Test0_Guid, new List<Guid>() { guidKey0, guidKey1 }), 2));
  ```
## Selecting data

Data query can be done on a single table or on several tables using one or more LEFT JOIN. The single table case is done using a SingleQueryContext object, with a parameter specifying the table to query. In addition, you have two optional parameters that you can mix freely:

1. A set of constraints specifying that either one or more colums must have a specific value, or that one column can have any of a collection of values.
2. A set of columns to return.

For example, the following operation queries values of *column_guid* for every rows where *column_int* is `2` or `3`.

  ```cs
  SelectResult = Database.Run(new SingleQueryContext(TestSchema.Test0, new ColumnValueCollectionPair<int>(TestSchema.Test0_Int, new List<int>() { 2, 3 }), new List<IColumnDescriptor>() { TestSchema.Test0_Guid }));
  ```

To read several tables, use a JoinQueryContext and connect columns of different tables with a dictionary:

  ```cs
  Dictionary<IColumnDescriptor, IColumnDescriptor> Join = new Dictionary<IColumnDescriptor, IColumnDescriptor>()
  {
    { TestSchema.Test1_Int, TestSchema.Test0_Int },
  };
  SelectResult = Database.Run(new JoinQueryContext(Join, new List<IColumnDescriptor>() { TestSchema.Test1_String, TestSchema.Test0_Guid }));
  ```

The example above returns values of *column_string* in table *test1*, and *column_guid* in table *test0*, where *column_int* is the same in both tables.
 

