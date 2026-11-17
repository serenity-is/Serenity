namespace Serenity.ComponentModel;

public class FormScriptAttributeTests()
{
    [Fact]
    public void Constructor_CanBeCalled_WithoutAnyKey()
    {
        var attribute = new FormScriptAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new FormScriptAttribute("key");
        Assert.Equal("key", attribute.Key);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => new FormScriptAttribute(null));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new FormScriptAttribute(""));
    }

    [Fact]
    public void AutoFormKeyFor_Strips_Root_And_Forms_Namespace()
    {
        Assert.Equal("Core.VersionInfo", FormScriptAttribute.AutoFormKeyFor(typeof(Acme.Core.Forms.VersionInfoForm)));
    }

    [Fact]
    public void AutoFormKeyFor_Strips_Columns_Namespace()
    {
        Assert.Equal("Core.VersionInfo", FormScriptAttribute.AutoFormKeyFor(typeof(Acme.Core.Columns.VersionInfoColumns)));
    }

    [Fact]
    public void AutoFormKeyFor_Keeps_Lookups_Namespace()
    {
        // only .Forms and .Columns are stripped; the codegen has to agree, see
        // ServerTypingsGeneratorTests.Form_Keeps_Lookups_Namespace
        Assert.Equal("Core.Lookups.CategoryLookup",
            FormScriptAttribute.AutoFormKeyFor(typeof(Acme.Core.Lookups.CategoryLookupForm)));
    }

    [Fact]
    public void AutoFormKeyFor_Strips_Request_Suffix()
    {
        Assert.Equal("Core.Save", FormScriptAttribute.AutoFormKeyFor(typeof(Acme.Core.Forms.SaveRequest)));
    }

    [Fact]
    public void AutoFormKeyFor_Uses_Module_Attribute_On_Type()
    {
        // [Module] wins over the namespace
        Assert.Equal("SomeModule.Module", FormScriptAttribute.AutoFormKeyFor(typeof(Acme.Core.Modules.ModuleForm)));
    }

    [Fact]
    public void AutoFormKeyFor_Uses_Module_Attribute_On_BasedOnRow_Row()
    {
        // the module comes from the row when the form has none
        Assert.Equal("SomeModule.Based", FormScriptAttribute.AutoFormKeyFor(typeof(Acme.Core.Modules.BasedForm)));
    }
}