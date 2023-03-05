using Serenity.CodeGenerator;
using static Serenity.Tests.MockModelInputs;

namespace Serenity.Tests.CodeGenerator;

public partial class EntiyModelGeneratorTests
{
    [Fact]
    public void Throws_ArgumentNull_If_Inputs_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityModelGenerator().GenerateModel(inputs: null));
    }

    [Fact]
    public void Uses_Passed_Input_Parameters()
    {
        var generator = new EntityModelGenerator();
        var inputs = new MockModelInputs();
        var model = generator.GenerateModel(inputs);
        Assert.Equal(Customer, model.ClassName);
        Assert.Equal(TestConnection, model.ConnectionKey);
        Assert.Equal(TestModule, model.Module);
        Assert.Equal(TestPermission, model.Permission);
        Assert.Equal(TestNamespace, model.RootNamespace);
        Assert.Equal(Customer + "Row", model.RowClassName);
        Assert.Equal(TestSchema, model.Schema);
        Assert.Equal(TestSchema, model.Schema);
        Assert.Equal(Customer, model.Title);
    }

    [Fact]
    public void DeclareJoinConstants_False_Configuration()
    {
        var generator = new EntityModelGenerator();
        var inputs = new MockModelInputs();
        inputs.Config.DeclareJoinConstants = false;
        var model = generator.GenerateModel(inputs);
        Assert.False(model.DeclareJoinConstants);
        
        var joinAttr = model.Fields.FirstOrDefault(x => x.Ident == "CityId")?
            .AttributeList?.FirstOrDefault(x => x.TypeName == "Serenity.Data.Mapping.LeftJoin");
        Assert.NotNull(joinAttr);
        Assert.Equal("\"jCity\"", joinAttr.Arguments);

        var cityJoin = model.Joins.FirstOrDefault(x => x.Name == "City");
        Assert.NotNull(cityJoin);
        
        var cityNameExpr = cityJoin.Fields.FirstOrDefault(x => x.Ident == "CityCityName")?
            .AttributeList?.FirstOrDefault(x => x.TypeName == "Serenity.Data.Mapping.Expression");
        Assert.NotNull(cityNameExpr);
        Assert.Equal("\"jCity.[CityName]\"", cityNameExpr.Arguments);

        var countryIdExpr = cityJoin.Fields.FirstOrDefault(x => x.Ident == "CityCountryId")?
            .AttributeList?.FirstOrDefault(x => x.TypeName == "Serenity.Data.Mapping.Expression");
        Assert.NotNull(countryIdExpr);
        Assert.Equal("\"jCity.[CountryId]\"", countryIdExpr.Arguments);
    }

    [Fact]
    public void DeclareJoinConstants_True_Configuration()
    {
        var generator = new EntityModelGenerator();
        var inputs = new MockModelInputs();
        inputs.Config.DeclareJoinConstants = true;
        var model = generator.GenerateModel(inputs);
        Assert.True(model.DeclareJoinConstants);

        var joinAttr = model.Fields.FirstOrDefault(x => x.Ident == "CityId")?
            .AttributeList?.FirstOrDefault(x => x.TypeName == "Serenity.Data.Mapping.LeftJoin");
        Assert.NotNull(joinAttr);
        Assert.Equal("jCity", joinAttr.Arguments);

        var cityJoin = model.Joins.FirstOrDefault(x => x.Name == "City");
        Assert.NotNull(cityJoin);

        var cityNameExpr = cityJoin.Fields.FirstOrDefault(x => x.Ident == "CityCityName")?
            .AttributeList?.FirstOrDefault(x => x.TypeName == "Serenity.Data.Mapping.Expression");
        Assert.NotNull(cityNameExpr);
        Assert.Equal("$\"{jCity}.[CityName]\"", cityNameExpr.Arguments);

        var countryIdExpr = cityJoin.Fields.FirstOrDefault(x => x.Ident == "CityCountryId")?
            .AttributeList?.FirstOrDefault(x => x.TypeName == "Serenity.Data.Mapping.Expression");
        Assert.NotNull(countryIdExpr);
        Assert.Equal("$\"{jCity}.[CountryId]\"", countryIdExpr.Arguments);
    }

}
