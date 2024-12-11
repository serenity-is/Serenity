namespace Serenity.ComponentModel;

public class DataScriptAttributeTests
{
    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new DataScriptAttribute("key");
        Assert.Equal("key", attribute.Key);
    }

    [Fact]
    public void Key_IsNull_ByDefault()
    {
        var attribute = new DataScriptAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void Constructor_ShouldThrow_ArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DataScriptAttribute(null));
    }

    [Fact]
    public void Permission_IsNull_ByDefault()
    {
        var attribute = new DataScriptAttribute();
        Assert.Null(attribute.Permission);
    }

    [Fact]
    public void Permission_CanBeSet_ToNull()
    {
        var attribute = new DataScriptAttribute()
        {
            Permission = null
        };
        Assert.Null(attribute.Permission);
    }

    [Fact]
    public void Permission_CanBeSet()
    {
        var attribute = new DataScriptAttribute()
        {
            Permission = "TestPermission"
        };
        Assert.Equal("TestPermission", attribute.Permission);
    }

    [Fact]
    public void CacheDuration_IsZero_ByDefault()
    {
        var attribute = new DataScriptAttribute();
        Assert.Equal(0, attribute.CacheDuration);
    }

    [Fact]
    public void CacheDuration_CanBeSet_ToInt()
    {
        var attribute = new DataScriptAttribute()
        {
            CacheDuration = 123
        };
        Assert.Equal(123, attribute.CacheDuration);
    }

    [Fact]
    public void CacheGroupKey_IsNull_ByDefault()
    {
        var attribute = new DataScriptAttribute();
        Assert.Null(attribute.CacheGroupKey);
    }

    [Fact]
    public void CacheGroupKey_CanBeSet()
    {
        var attribute = new DataScriptAttribute()
        {
            CacheGroupKey = "TestCacheKey"
        };
        Assert.Equal("TestCacheKey", attribute.CacheGroupKey);
    }

    [Fact]
    public void CacheGroupKey_CanBeSet_ToNull()
    {
        var attribute = new DataScriptAttribute()
        {
            CacheGroupKey = null
        };
        Assert.Null(attribute.CacheGroupKey);
    }

    [Fact]
    public void AutoKeyFor_ReturnsCorrectKey()
    {
        var type = typeof(MyClassWithModule);

        var key = DataScriptAttribute.AutoKeyFor(type);

        Assert.Equal("TestModule.MyClassWithModule", key);
    }

    [Fact]
    public void AutoKeyFor_Returns_TypeName_When_ModuleAttribute_NotPresent()
    {
        var type = typeof(MyClassWithoutModule);
        var result = DataScriptAttribute.AutoKeyFor(type);
        Assert.Equal("ComponentModel.MyClassWithoutModule", result);
    }

    [Fact]
    public void AutoKeyFor_Handles_Namespace_With_SubNamespace()
    {
        var type = typeof(MyClassWithModule);
        var result = DataScriptAttribute.AutoKeyFor(type);
        Assert.Equal("TestModule.MyClassWithModule", result);
    }

    [Module("TestModule")]
    public class MyClassWithModule{ }

    public class MyClassWithoutModule { }
}
