using static Serenity.CodeGenerator.CustomerEntityInputs;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    [Fact]
    public void Field_Selection_None_Keeps_Only_Included_Foreign_Fields()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Config.ForeignFieldSelection = GeneratorConfig.FieldSelection.None;
        inputs.Config.IncludeForeignFields = [CityName];

        var model = new EntityModelFactory().Create(inputs);

        var cityJoin = Assert.Single(model.Joins, j => j.Name == City);
        Assert.Equal([CityName], cityJoin.Fields.Select(x => x.PropertyName));
        Assert.Equal(CityId, cityJoin.SourceField);

        var cityId = Assert.Single(model.Fields, x => x.PropertyName == CityId);
        Assert.Equal(CityName, cityId.TextualField);
    }

    [Fact]
    public void Field_Selection_NameOnly_Keeps_Name_Field_And_Textual_Reference()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Config.ForeignFieldSelection = GeneratorConfig.FieldSelection.NameOnly;

        var model = new EntityModelFactory().Create(inputs);

        var cityJoin = Assert.Single(model.Joins, j => j.Name == City);
        var cityField = Assert.Single(cityJoin.Fields, x => x.PropertyName == CityName);
        Assert.Equal(cityField.PropertyName, Assert.Single(model.Fields, x => x.PropertyName == CityId).TextualField);
        Assert.Contains(cityField.AttributeList, a => a.TypeName == "Serenity.Data.Mapping.Expression");
    }

    [Fact]
    public void RemoveForeignFields_Drops_Security_Sensitive_And_Configured_Fields()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Config.RemoveForeignFields = [CityName, "   ", "Pass"];

        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, City, F("Pass", "nvarchar", size: 50));

        var model = new EntityModelFactory().Create(inputs);

        var cityJoin = Assert.Single(model.Joins, j => j.Name == City);
        Assert.Equal([CityCountryId], cityJoin.Fields.Select(x => x.PropertyName));
        Assert.Null(Assert.Single(model.Fields, x => x.PropertyName == CityId).TextualField);
    }

    [Fact]
    public void Default_Selection_Adds_Omittable_Fields_With_Display_Name_And_Expression()
    {
        var inputs = new CustomerEntityInputs();

        var model = new EntityModelFactory().Create(inputs);

        var cityJoin = Assert.Single(model.Joins, j => j.Name == City);
        Assert.Equal(CityId, cityJoin.SourceField);

        var cityCountryId = Assert.Single(cityJoin.Fields, x => x.PropertyName == CityCountryId);
        Assert.Equal("City Country Id", cityCountryId.Title);
        Assert.Contains(cityCountryId.AttributeList, a =>
            a.TypeName == "System.ComponentModel.DisplayName" &&
            "City Country Id".Equals(a.Arguments[0]));
        Assert.Contains(cityCountryId.AttributeList, a =>
            a.TypeName == "Serenity.Data.Mapping.Expression" &&
            "jCity.[CountryId]".Equals(a.Arguments[0]));
    }

    [Fact]
    public void Omit_Default_Schema_Normalizes_Foreign_Key_Schema_Too()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "User";
        inputs.Schema = "dbo";
        inputs.Config.OmitDefaultSchema = true;
        var schema = (CustomerDataSchema)inputs.DataSchema;
        schema.FieldInfos.Add(("dbo", "User", F("UserId", "int", pk: true, identity: true, nullable: false)));
        schema.FieldInfos.Add(("dbo", "User", F("UserCityId", "int", nullable: true)));
        schema.ForeignKeys.Add(("dbo", "User", new()
        {
            FKName = "FK_User_CityId",
            FKColumn = "UserCityId",
            PKSchema = "dbo",
            PKTable = City,
            PKColumn = "CityId",
        }));

        var model = new EntityModelFactory().Create(inputs);

        var userCityId = Assert.Single(model.Fields, x => x.PropertyName == "UserCityId");
        Assert.Null(userCityId.PKSchema);
        Assert.Contains(userCityId.AttributeList, a =>
            a.TypeName == "Serenity.Data.Mapping.ForeignKey" &&
            City.Equals(a.Arguments[0]));
    }
}
