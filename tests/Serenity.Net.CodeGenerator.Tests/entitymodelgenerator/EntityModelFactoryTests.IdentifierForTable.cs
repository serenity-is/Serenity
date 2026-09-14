using static Serenity.CodeGenerator.CustomerEntityInputs;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    [Theory]
    [InlineData("Customer", "Customer")]
    [InlineData("tb_Customer", "Customer")]
    [InlineData("aspnet_Users", "AspNetUsers")]
    [InlineData("AspNetMembership", "AspNetMembership")]
    [InlineData("Some Table Name", "SomeTableName")]
    [InlineData("my_category", "MyCategory")]
    public void IdentifierForTable_Strips_Prefixes_and_Pascalizes(string tableName, string expected)
    {
        Assert.Equal(expected, EntityModelFactory.IdentifierForTable(tableName));
    }

    private class TableInputs : CustomerEntityInputs
    {
        public TableInputs(string table, string identifier)
        {
            Identifier = identifier;
            Schema = TestSchema;
            Table = table;
        }
    }
}
