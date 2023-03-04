
using Serenity.CodeGenerator;

namespace Serenity.Tests;

public class MockModelInputs : EntityModelInputs
{
    public const string TestSchema = "test";
    public const string TestConnection = "TestConnection";
    public const string TestModule = "TestModule";
    public const string TestPermission = "TestPermission";
    public const string TestNamespace = "TestNamespace";

    public const string Customer = "Customer";
    public const string City = "City";
    public const string Country = "Country";

    public MockModelInputs()
    {
        Identifier = Customer;
        Schema = TestSchema;
        Table = Customer;
        Config = new GeneratorConfig() 
        { 
            RootNamespace = TestNamespace,
            EndOfLine = "lf"
        };
        ConnectionKey = TestConnection;
        DataSchema = new MockDataSchema();
        Module = TestModule;
        Net5Plus = true;
        PermissionKey = TestPermission;
    }
}