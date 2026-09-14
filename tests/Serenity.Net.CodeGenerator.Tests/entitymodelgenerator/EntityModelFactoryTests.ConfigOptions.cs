using static Serenity.CodeGenerator.CustomerEntityInputs;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    [Fact]
    public void Config_Options_Are_Mapped_To_Model()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Customer";
        inputs.Config.DeclareJoinConstants = true;
        inputs.Config.EnableGenerateFields = true;
        inputs.Config.EnableRowTemplates = true;
        inputs.Config.FileScopedNamespaces = true;
        inputs.Config.GenerateUI = false;
        inputs.Net8Plus = false;
        inputs.GlobalUsings.Add("System");
        inputs.GlobalUsings.Add("TestApp.Usings");

        var model = new EntityModelFactory().Create(inputs);

        Assert.True(model.DeclareJoinConstants);
        Assert.True(model.EnableGenerateFields);
        Assert.True(model.EnableGenerateInterface);
        Assert.True(model.EnableRowTemplates);
        Assert.True(model.FileScopedNamespaces);
        Assert.False(model.GenerateListExcel);
        Assert.False(model.NET8Plus);
        Assert.Contains(inputs.GlobalUsings, model.GlobalUsings.Contains);
    }

    [Fact]
    public void Omit_Default_Schema_Removes_Schema_When_Equal_To_Data_Schema_Default()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Customer";
        inputs.Schema = "dbo";
        inputs.Config.OmitDefaultSchema = true;

        var model = new EntityModelFactory().Create(inputs);
        Assert.Null(model.Schema);

        inputs.Schema = TestSchema;
        var withDifferentSchema = new EntityModelFactory().Create(inputs);
        Assert.Equal(TestSchema, withDifferentSchema.Schema);
    }

    [Fact]
    public void Schema_Is_Database_Always_Omits_Schema()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Customer";
        inputs.SchemaIsDatabase = true;

        var model = new EntityModelFactory().Create(inputs);
        Assert.Null(model.Schema);
    }

    [Fact]
    public void Auto_Identifier_Is_Derived_When_Identifier_Is_Deleted()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "tb_Customer";
        inputs.Identifier = null;

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal("Customer", model.ClassName);
        Assert.Equal("Customer" + "Row", model.RowClassName);
        Assert.Equal("Customer", model.Title);
    }
}
