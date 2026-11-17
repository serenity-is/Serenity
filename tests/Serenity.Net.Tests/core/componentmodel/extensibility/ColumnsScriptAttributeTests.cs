namespace Serenity.ComponentModel;

public class ColumnsScriptAttributeTests
{
    [Fact]
    public void ColumnsScriptAttribute_CanBeCreated_WithoutAnyArgs()
    {
        _ = new ColumnsScriptAttribute();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenKeyArgument_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new ColumnsScriptAttribute(null));
        Assert.Throws<ArgumentNullException>(() => new ColumnsScriptAttribute(""));
    }

    [Fact]
    public void Key_IsNull_ByDefault()
    {
        var attribute = new ColumnsScriptAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new ColumnsScriptAttribute("someKey");
        Assert.Equal("someKey", attribute.Key);
    }

    [Fact]
    public void LocalTextPrefix_IsNull_ByDefault()
    {
        var attribute = new ColumnsScriptAttribute();
        Assert.Null(attribute.LocalTextPrefix);
    }

    [Fact]
    public void LocalTextPrefix_CanBeSet()
    {
        var attribute = new ColumnsScriptAttribute
        {
            LocalTextPrefix = "TestPrefix"
        };
        Assert.Equal("TestPrefix", attribute.LocalTextPrefix);
    }

    [Fact]
    public void AutoColumnsKeyFor_Strips_Root_And_Columns_Namespace()
    {
        Assert.Equal("Core.VersionInfo",
            ColumnsScriptAttribute.AutoColumnsKeyFor(typeof(Acme.Core.Columns.VersionInfoColumns)));
    }

    [Fact]
    public void AutoColumnsKeyFor_Infers_The_Same_Key_As_AutoFormKeyFor()
    {
        // a columns key is inferred exactly like a form key, see the attribute docs
        var type = typeof(Acme.Core.Columns.VersionInfoColumns);
        Assert.Equal(FormScriptAttribute.AutoFormKeyFor(type), ColumnsScriptAttribute.AutoColumnsKeyFor(type));
    }

    [Fact]
    public void AutoColumnsKeyFor_Uses_Module_Attribute_On_Type()
    {
        Assert.Equal("SomeModule.Module",
            ColumnsScriptAttribute.AutoColumnsKeyFor(typeof(Acme.Core.Modules.ModuleForm)));
    }
}