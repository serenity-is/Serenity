using static Serenity.CodeGenerator.CustomerEntityInputs;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    [Fact]
    public void Service_Lookup_Permission_Is_Set_For_Single_PK_Table()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Account";
        inputs.Identifier = "Account";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Account", F("AccountId", "int", pk: true, identity: true, nullable: false));
        AddField(schema, "Account", F("AccountName", "nvarchar", nullable: false, size: 50));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Equal(TestPermission, model.ServiceLookupPermission);
    }

    [Fact]
    public void Service_Lookup_Permission_Is_Skipped_For_Multiple_PK_Table()
    {
        var inputs = new CustomerEntityInputs();
        inputs.Table = "Account";
        inputs.Identifier = "Account";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Account", F("AccountId", "int", pk: true, identity: true, nullable: false));
        AddField(schema, "Account", F("AccountId2", "int", pk: true, nullable: false));
        AddField(schema, "Account", F("AccountName", "nvarchar", nullable: false, size: 50));

        var model = new EntityModelFactory().Create(inputs);

        Assert.Null(model.ServiceLookupPermission);
    }

    private static TestRowMetadata CityRowDeserialize()
    {
        var cityRow = new TestRowMetadata
        {
            Namespace = "TestApp.Entities",
            ClassName = "CityRow",
            Module = "TestModule",
            IdProperty = "CityId",
            NameProperty = "CityTitle"
        };
        cityRow.Props.Add(new() { PropertyName = "CityId", ColumnName = "CityId" });
        cityRow.Props.Add(new() { PropertyName = "CityTitle", ColumnName = "TitleCol" });
        return cityRow;
    }

    [Fact]
    public void Name_Property_Missing_From_Foreign_Fields_Creates_Origin_Textual_Field()
    {
        var cityRow = CityRowDeserialize();
        var app = new TestApplicationMetadata();
        app.Rows["City"] = cityRow;
        app.Rows["[test].[City]"] = cityRow;

        var inputs = new CustomerEntityInputs { Application = app };
        inputs.Config.DeclareJoinConstants = false;

        var model = new EntityModelFactory().Create(inputs);

        var cityId = Assert.Single(model.Fields, x => x.PropertyName == CityId);
        Assert.Equal("CityTitle", cityId.TextualField);

        var cityJoin = Assert.Single(model.Joins, j => j.Name == City);
        var titleField = Assert.Single(cityJoin.Fields, x => x.PropertyName == "CityTitle");
        Assert.Equal("TitleCol", titleField.Name);
        Assert.Equal("City Title Col", titleField.Title);
        Assert.Contains(titleField.AttributeList, a =>
            a.TypeName == "System.ComponentModel.DisplayName" &&
            "City Title Col".Equals(a.Arguments[0]));
        var origin = Assert.Single(titleField.AttributeList, a => a.TypeName == "Serenity.Data.Mapping.Origin");
        Assert.Equal(2, origin.Arguments.Length);
        Assert.Equal("jCity", Assert.IsType<string>(origin.Arguments[0]));
        var nameOf = Assert.IsType<NameOfRef>(origin.Arguments[1]);
        Assert.Equal("TestApp.Entities.CityRow", nameOf.TypeName);
        Assert.Equal("CityTitle", nameOf.PropertyName);
    }

    [Fact]
    public void Foreign_Field_With_Row_Uses_Origin_Instead_Of_Expression()
    {
        var cityRow = new TestRowMetadata
        {
            Namespace = "TestApp.Entities",
            ClassName = "CityRow",
            IdProperty = "CityId"
        };
        cityRow.Props.Add(new() { PropertyName = "CityId", ColumnName = "CityId" });
        cityRow.Props.Add(new() { PropertyName = "CityCountryName", ColumnName = "CountryName" });

        var app = new TestApplicationMetadata();
        app.Rows["Country"] = cityRow;
        app.Rows["[test].[Country]"] = cityRow;

        var inputs = new CustomerEntityInputs { Application = app };
        inputs.Table = "City";
        inputs.Identifier = "City";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        schema.ForeignKeys.Add((TestSchema, City, new()
        {
            FKName = "FK_City_CountryId",
            FKColumn = "CountryId",
            PKSchema = TestSchema,
            PKTable = Country,
            PKColumn = CountryId,
        }));

        var model = new EntityModelFactory().Create(inputs);

        var join = Assert.Single(model.Joins, j => j.Name == Country);
        var field = Assert.Single(join.Fields, x => x.Name == "CountryName");
        var origin = Assert.Single(field.AttributeList, a => a.TypeName == "Serenity.Data.Mapping.Origin");
        var nameOf = Assert.IsType<NameOfRef>(origin.Arguments[1]);
        Assert.Equal("TestApp.Entities.CityRow", nameOf.TypeName);
        Assert.Equal("CityCountryName", nameOf.PropertyName);
        Assert.DoesNotContain(field.AttributeList, a => a.TypeName == "Serenity.Data.Mapping.Expression");
    }

    [Fact]
    public void Numeric_Name_Property_Can_Be_Textual_Field_For_Foreign_Join()
    {
        var numRow = new TestRowMetadata
        {
            Namespace = "TestApp.Entities",
            ClassName = "NumRow",
            IdProperty = "NumId",
            NameProperty = "NumValue"
        };
        numRow.Props.Add(new() { PropertyName = "NumId", ColumnName = "NumId" });
        numRow.Props.Add(new() { PropertyName = "NumValue", ColumnName = "NumValue" });

        var app = new TestApplicationMetadata();
        app.Rows["Num"] = numRow;
        app.Rows["[test].[Num]"] = numRow;

        var inputs = new CustomerEntityInputs { Application = app };
        inputs.Table = "City";
        inputs.Identifier = "City";
        var schema = (CustomerDataSchema)inputs.DataSchema;
        AddField(schema, "Num", F("NumId", "int", pk: true, nullable: false));
        AddField(schema, "Num", F("NumValue", "int"));
        schema.ForeignKeys.Add((TestSchema, City, new()
        {
            FKName = "FK_City_NumId",
            FKColumn = "NumId",
            PKSchema = TestSchema,
            PKTable = "Num",
            PKColumn = "NumId",
        }));
        AddField(schema, City, F("NumId", "int", pk: true, nullable: false));

        var model = new EntityModelFactory().Create(inputs);

        var join = Assert.Single(model.Joins, j => j.Name == "Num");
        var numValueField = Assert.Single(join.Fields, x => x.PropertyName == "NumValue");
        Assert.Equal(numValueField.PropertyName,
            Assert.Single(model.Fields, x => x.Name == "NumId").TextualField);
    }
}
