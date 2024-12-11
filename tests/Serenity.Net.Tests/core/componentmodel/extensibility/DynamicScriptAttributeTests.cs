namespace Serenity.ComponentModel;

public class DynamicScriptAttributeTests
{
    public class MyDynamicScriptAttribute : DynamicScriptAttribute
    {
        public MyDynamicScriptAttribute()
            : base("key")
        {
        }
    }

    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new MyDynamicScriptAttribute();
        Assert.Equal("key", attribute.Key);
    }

    [Fact]
    public void CacheDuration_CanBeSet_ToInt()
    {
        var attribute = new MyDynamicScriptAttribute()
        {
            CacheDuration = 123
        };
        Assert.Equal(123, attribute.CacheDuration);
    }

    [Fact]
    public void CacheDuration_IsZero_ByDefault()
    {
        var attribute = new MyDynamicScriptAttribute();
        Assert.Equal(0, attribute.CacheDuration);
    }

    [Fact]
    public void CacheGroupKey_IsNull_ByDefault()
    {
        var attribute = new MyDynamicScriptAttribute();
        Assert.Null(attribute.CacheGroupKey);
    }

    [Fact]
    public void CacheGroupKey_CanBeSet()
    {
        var attribute = new MyDynamicScriptAttribute()
        {
            CacheGroupKey = "somekey"
        };
        Assert.Equal("somekey", attribute.CacheGroupKey);
    }

    [Fact]
    public void CacheGropKey_CanBeSet_ToNull()
    {
        var attribute = new MyDynamicScriptAttribute
        {
            CacheGroupKey = null
        };
        Assert.Null(attribute.CacheGroupKey);
    }
}



