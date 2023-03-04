using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class EntiyModelGeneratorTests
{
    [Fact]
    public void Throws_ArgumentNull_If_Inputs_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityModelGenerator().GenerateModel(inputs: null));
    }

    const string testIdentifier = "Test";
    const string testSchema = "dbo";
    const string testTable = "test";
    const string testConnection = $"{testIdentifier}TestConnection";
    const string testModule = $"{testIdentifier}Module";
    const string testPermission = $"{testIdentifier}Permission";
    const string testRowClass = $"{testIdentifier}Row";
    const string testTitle = testIdentifier;
    const string testRootNamespace = $"My{testIdentifier}";

    private EntityModelInputs CreateMockInputs()
    {
        return new EntityModelInputs
        {
            Identifier = testIdentifier,
            Schema = testSchema,
            Table = testTable,
            Config = new GeneratorConfig() { RootNamespace = "MyTest"},
            ConnectionKey = testConnection,
            DataSchema = new MockDataSchema(),
            Module = testModule,
            Net5Plus = true,
            PermissionKey = testPermission,
        };
    }

    [Fact]
    public void Uses_Passed_Input_Parameters()
    {
        var generator = new EntityModelGenerator();
        var inputs = CreateMockInputs();
        var model = generator.GenerateModel(inputs);
        Assert.Equal(testIdentifier, model.ClassName);
        Assert.Equal(testConnection, model.ConnectionKey);
        Assert.Equal(testModule, model.Module);
        Assert.Equal(testPermission, model.Permission);
        Assert.Equal(testRootNamespace, model.RootNamespace);
        Assert.Equal(testRowClass, model.RowClassName);
        Assert.Equal(testSchema, model.Schema);
        Assert.Equal(testSchema, model.Schema);
        Assert.Equal(testTitle, model.Title);
    }
}
