using static Serenity.CodeGenerator.CustomerEntityInputs;

namespace Serenity.CodeGenerator;

public partial class EntityModelFactoryTests
{
    private static TestApplicationMetadata AppWithCity(bool hasLookup, string? route = null)
    {
        var cityRow = new TestRowMetadata
        {
            Namespace = "TestApp.Entities",
            ClassName = "CityRow",
            Module = "TestModule",
            IdProperty = "CityId",
            NameProperty = "CityName",
            HasLookupScriptAttribute = hasLookup,
            ListServiceRoute = route
        };
        cityRow.Props.Add(new() { PropertyName = "CityId", ColumnName = "CityId" });
        cityRow.Props.Add(new() { PropertyName = "CityName", ColumnName = "CityName" });

        var app = new TestApplicationMetadata();
        app.Rows["City"] = cityRow;
        app.Rows["[test].[City]"] = cityRow;
        return app;
    }

    [Fact]
    public void Application_Rows_Produce_Typed_Foreign_Key_And_Lookup_Editor()
    {
        var inputs = new CustomerEntityInputs { Application = AppWithCity(hasLookup: true) };

        var model = new EntityModelFactory().Create(inputs);

        var cityId = Assert.Single(model.Fields, x => x.PropertyName == CityId);
        Assert.Contains(cityId.AttributeList, a =>
            a.TypeName == "Serenity.Data.Mapping.ForeignKey" &&
            a.Arguments[0] is TypeOfRef && a.Arguments.Length == 1);
        Assert.Contains(cityId.AttributeList, a =>
            a.TypeName == "Serenity.ComponentModel.LookupEditor" &&
            a.Arguments[0] is TypeOfRef && a.Arguments[1] is RawCode raw &&
            raw.Code == "Async = true");
        Assert.Contains(cityId.AttributeList, a =>
            a.TypeName == "Serenity.Data.Mapping.LeftJoin" &&
            "jCity".Equals(a.Arguments[0]));
    }

    [Fact]
    public void Default_Route_Uses_Relative_Service_Editor_Argument()
    {
        var inputs = new CustomerEntityInputs
        {
            Application = AppWithCity(hasLookup: false, route: "Services/TestModule/City/List")
        };

        var model = new EntityModelFactory().Create(inputs);

        var cityId = Assert.Single(model.Fields, x => x.PropertyName == CityId);
        var editor = Assert.Single(cityId.AttributeList,
            a => a.TypeName == "Serenity.ComponentModel.ServiceLookupEditor");
        Assert.Equal(2, editor.Arguments.Length);
        var service = Assert.IsType<RawCode>(editor.Arguments[1]).Code;
        Assert.Contains("TestModule/City/List", service);
        Assert.DoesNotContain("~/", service);
    }

    [Fact]
    public void Non_Default_Relative_Route_Is_Used_As_Service_Editor_Service_Argument()
    {
        var inputs = new CustomerEntityInputs
        {
            Application = AppWithCity(hasLookup: false, route: "Services/Other/City/List")
        };

        var model = new EntityModelFactory().Create(inputs);

        var cityId = Assert.Single(model.Fields, x => x.PropertyName == CityId);
        var editor = Assert.Single(cityId.AttributeList,
            a => a.TypeName == "Serenity.ComponentModel.ServiceLookupEditor");
        Assert.Single(editor.Arguments);
        Assert.IsType<TypeOfRef>(editor.Arguments[0]);
    }

    [Fact]
    public void Route_Outside_Services_Folder_Gets_Tilde_Prefix()
    {
        var inputs = new CustomerEntityInputs
        {
            Application = AppWithCity(hasLookup: false, route: "Special/City/List")
        };

        var model = new EntityModelFactory().Create(inputs);

        var cityId = Assert.Single(model.Fields, x => x.PropertyName == CityId);
        var editor = Assert.Single(cityId.AttributeList,
            a => a.TypeName == "Serenity.ComponentModel.ServiceLookupEditor");
        Assert.Single(editor.Arguments);
        Assert.IsType<TypeOfRef>(editor.Arguments[0]);
    }

    [Fact]
    public void Missing_Route_Means_No_Editor_Attribute()
    {
        var inputs = new CustomerEntityInputs { Application = AppWithCity(hasLookup: false) };

        var model = new EntityModelFactory().Create(inputs);

        var cityId = Assert.Single(model.Fields, x => x.PropertyName == CityId);
        Assert.DoesNotContain(cityId.AttributeList, a =>
            a.TypeName == "Serenity.ComponentModel.ServiceLookupEditor");
        Assert.DoesNotContain(cityId.AttributeList, a =>
            a.TypeName == "Serenity.ComponentModel.LookupEditor");
    }
}
