#pragma warning disable CS0649
using Serenity.PropertyGrid;

namespace Serenity.Web;

public class PropertyItemsScriptTests
{
    private class TestProvider : IPropertyItemProvider
    {
        public Func<Type, Func<PropertyInfo, bool>?, IEnumerable<PropertyItem>> Factory { get; set; } = (_, _) => [];

        public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type, Func<PropertyInfo, bool>? predicate = null)
            => Factory(type, predicate);
    }

    private class CustomizedType : ICustomizePropertyItems
    {
        public void Customize(List<PropertyItem> input)
        {
            input.Add(new PropertyItem { Name = "Custom" });
        }
    }

    [TableName("BasedOnRows")]
    private class BasedOnRow : Row<BasedOnRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [BasedOnRow(typeof(BasedOnRow))]
    private class BasedOnType
    {
    }

    private static (FormScript script, TestProvider provider) CreateFormScript(Type type,
        Func<Type, Func<PropertyInfo, bool>?, IEnumerable<PropertyItem>>? factory = null)
    {
        var provider = new TestProvider();
        if (factory != null)
            provider.Factory = factory;
        var services = new ServiceCollection().BuildServiceProvider();
        return (new FormScript("Test", type, provider, services), provider);
    }

    [Fact]
    public void Compact_Throws_ForNullInputs()
    {
        Assert.Throws<ArgumentNullException>(() => PropertyItemsScript.Compact(null!));
    }

    [Fact]
    public void Compact_SerializesBasicProperties()
    {
        var item = new PropertyItem
        {
            Name = "A",
            Title = "Long Title",
            EditorType = "String",
            FilteringType = "String",
            Required = true,
            ReadOnly = false,
            MaxLength = 50,
            SortOrder = 3,
            DefaultValue = 1.5m,
            EditorCssClass = "a.b/c_d:e:value",
            Tab = "tab"
        };

        var result = PropertyItemsScript.Compact([
            ("Form.Test", new PropertyItemsData { Items = [item], AdditionalItems = [] })
        ]);

        Assert.Contains("Serenity.setScriptData", result, StringComparison.Ordinal);
        Assert.Contains("!0", result, StringComparison.Ordinal);
        Assert.Contains("!1", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Compact_HandlesNullItemEmptyDictionaryAndExtensionData()
    {
        var item = new PropertyItem { Name = "A" };
        item.ExtensionData["a-b"] = 1;
        item.ExtensionData["longkey"] = new Dictionary<string, object?> { ["k"] = "longvalue" };
        item.ExtensionData["emptyDict"] = new Dictionary<string, object?>();
        item.ExtensionData["list"] = new List<object?> { 1, "two", true };
        item.ExtensionData["date"] = new DateTime(2024, 1, 1);

        var result = PropertyItemsScript.Compact([
            ("Form.Test", new PropertyItemsData { Items = [item, null!], AdditionalItems = null })
        ]);

        Assert.NotNull(result);
    }

    [Fact]
    public void Compact_WithAdditionalItemsAndMultipleInputs()
    {
        var additional = new PropertyItem { Name = "Extra", Title = "ExtraTitleValue" };
        var result = PropertyItemsScript.Compact([
            ("Form.A", new PropertyItemsData { Items = [new PropertyItem { Name = "A" }], AdditionalItems = [additional] }),
            ("Form.B", new PropertyItemsData { Items = [new PropertyItem { Name = "B" }], AdditionalItems = [] })
        ]);

        Assert.NotNull(result);
        Assert.Contains("additionalItems", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Compact_WithManyUniqueStrings_CoversKeyGeneration()
    {
        var items = new List<PropertyItem>();
        for (var i = 0; i < 60; i++)
            items.Add(new PropertyItem { Name = "P" + i, Title = "UniqueTitleValue" + i });

        var result = PropertyItemsScript.Compact([
            ("Form.Test", new PropertyItemsData { Items = items, AdditionalItems = [] })
        ]);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetScriptData_ReturnsProvidedItems()
    {
        var (script, _) = CreateFormScript(typeof(object), (_, _) =>
            [new PropertyItem { Name = "A" }]);

        var data = Assert.IsType<PropertyItemsData>(script.GetScriptData());
        Assert.Single(data.Items!);
    }

    [Fact]
    public void GetScriptData_CustomizesItems_WhenTypeImplementsInterface()
    {
        var (script, _) = CreateFormScript(typeof(CustomizedType), (_, _) => [new PropertyItem { Name = "A" }]);

        var data = Assert.IsType<PropertyItemsData>(script.GetScriptData());

        Assert.Contains(data.Items!, x => x.Name == "Custom");
    }

    [Fact]
    public void GetScriptData_AddsBasedOnRowAdditionalItems()
    {
        var (script, _) = CreateFormScript(typeof(BasedOnType), (type, predicate) =>
        {
            if (predicate != null)
                return [new PropertyItem { Name = "BasedField" }];
            return [new PropertyItem { Name = "A", FilteringIdField = "BasedField" }];
        });

        var data = Assert.IsType<PropertyItemsData>(script.GetScriptData());

        Assert.NotNull(data.AdditionalItems);
        Assert.Contains(data.AdditionalItems!, x => x.Name == "BasedField");
    }

    [Fact]
    public void GetScript_FormatsSetScriptData()
    {
        var (script, _) = CreateFormScript(typeof(object), (_, _) => [new PropertyItem { Name = "A" }]);

        var text = script.GetScript();

        Assert.Contains("Serenity.setScriptData", text, StringComparison.Ordinal);
        Assert.Contains("Form.Test", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Changed_RaisesScriptChanged()
    {
        var (script, _) = CreateFormScript(typeof(object));
        var raised = false;
        script.ScriptChanged += (_, _) => raised = true;

        script.Changed();

        Assert.True(raised);
    }

    [Fact]
    public void FormScript_Throws_ForEmptyName()
    {
        var provider = new TestProvider();
        var services = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => new FormScript("", typeof(object), provider, services));
    }

    [Fact]
    public void FormScript_Throws_ForNullProvider()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => new FormScript("Test", typeof(object), null!, services));
    }

    [Fact]
    public void ColumnsScript_Throws_ForNullType()
    {
        var provider = new TestProvider();
        var services = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => new ColumnsScript("Test", null!, provider, services));
    }

    [Fact]
    public void ColumnsScript_SetsPrefixedName()
    {
        var provider = new TestProvider();
        var services = new ServiceCollection().BuildServiceProvider();
        var script = new ColumnsScript("Test", typeof(object), provider, services);
        Assert.Equal("Columns.Test", script.ScriptName);
    }

    [Fact]
    public void CheckRights_DoesNothing()
    {
        var (script, _) = CreateFormScript(typeof(object));
        script.CheckRights(new MockPermissions(_ => false), NullTextLocalizer.Instance);
    }

    [Fact]
    public void ExpirationAndGroupKey_AreSettable()
    {
        var (script, _) = CreateFormScript(typeof(object));
        script.Expiration = TimeSpan.FromMinutes(5);
        script.GroupKey = "G";
        Assert.Equal(TimeSpan.FromMinutes(5), script.Expiration);
        Assert.Equal("G", script.GroupKey);
    }
}

