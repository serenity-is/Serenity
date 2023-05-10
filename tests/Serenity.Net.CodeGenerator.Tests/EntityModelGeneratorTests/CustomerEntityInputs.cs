
using Serenity.CodeGenerator;

namespace Serenity.Tests;

public class CustomerEntityInputs : EntityModelInputs
{
    public const string TestSchema = "test";
    public const string TestConnection = nameof(TestConnection);
    public const string TestModule = nameof(TestModule);
    public const string TestPermission = nameof(TestPermission);
    public const string TestNamespace = nameof(TestNamespace);

    public const string Customer = nameof(Customer);
    public const string CustomerId = nameof(CustomerId);
    public const string CustomerName = nameof(CustomerName);
    public const string City = nameof(City);
    public const string Country = nameof(Country);
    public const string CountryId = nameof(CountryId);
    public const string CityId = nameof(CityId);
    public const string CityName = nameof(CityName);
    public const string CityCountryId = nameof(CityCountryId);

    public const string jCity = nameof(jCity);

    public const string ExpressionAttrName = "Serenity.Data.Mapping.Expression";
    public const string LeftJoinAttrName = "Serenity.Data.Mapping.LeftJoin";

    public CustomerEntityInputs()
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
        DataSchema = new CustomerDataSchema();
        Module = TestModule;
        Net5Plus = true;
        PermissionKey = TestPermission;
    }
}