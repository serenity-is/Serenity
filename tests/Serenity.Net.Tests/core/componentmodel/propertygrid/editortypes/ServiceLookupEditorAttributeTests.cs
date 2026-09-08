namespace Serenity.ComponentModel;

public class ServiceLookupEditorAttributeTests
{
    private class TestRow
    {
    }

    [Module("Northwind")]
    private class ModuleRow
    {
    }

    private class EntitiesRow
    {
    }

    private class MyLookup
    {
    }

    [Fact]
    public void Ctor_WithServiceIdAndTextField_SetsOptions()
    {
        var attr = new ServiceLookupEditorAttribute("Northwind/Customer/List", "ID", "Name");

        var dict = new Dictionary<string, object?>();
        attr.SetParams(dict);

        Assert.Equal("Northwind/Customer/List", dict["service"]);
        Assert.Equal("ID", dict["idField"]);
        Assert.Equal("Name", dict["textField"]);
    }

    [Fact]
    public void Ctor_WithItemType_SetsItemType()
    {
        var attr = new ServiceLookupEditorAttribute(typeof(TestRow));
        Assert.Equal(typeof(TestRow), attr.ItemType);
    }

    [Fact]
    public void Ctor_WithNullItemType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceLookupEditorAttribute(null));
    }

    [Fact]
    public void AutoServiceFor_UsesModuleAttribute()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(typeof(ModuleRow));
        Assert.Equal("Northwind/Module/List", service);
    }

    [Fact]
    public void AutoServiceFor_RemovesEntitiesSuffix()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(typeof(EntitiesRow));
        Assert.Equal("ComponentModel/Entities/List", service);
    }

    [Fact]
    public void AutoServiceFor_RemovesLookupSuffix()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(typeof(MyLookup));
        Assert.Equal("ComponentModel/My/List", service);
    }
}