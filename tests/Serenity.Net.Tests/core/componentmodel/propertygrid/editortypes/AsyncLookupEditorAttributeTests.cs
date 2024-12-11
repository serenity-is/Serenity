namespace Serenity.ComponentModel;

public class AsyncLookupEditorAttributeTests
{
    public class TestClass
    {
    }

    public class TestCustomEditorAttribute : AsyncLookupEditorAttribute
    {
        public TestCustomEditorAttribute(string lookupKey) : base(lookupKey)
        {
        }

        public TestCustomEditorAttribute(Type lookupType) : base(lookupType.FullName)
        {
        }

        public void SetOptionPublic(string key, object value)
        {
            SetOption(key, value);
        }

        public T GetOptionPublic<T>(string key)
        {
            return GetOption<T>(key);
        }
    }

    [Fact]
    public void SetOptionAsync_True_SetsAsyncOption()
    {
        var attribute = new TestCustomEditorAttribute("lookupKey");
        attribute.SetOptionPublic("async", true);
        Assert.True(attribute.GetOptionPublic<bool>("async"));
    }

    [Fact]
    public void ConstructorWithLookupKey_SetsAsyncOption()
    {
        var attribute = new TestCustomEditorAttribute("lookupKey");
        var asyncOption = attribute.GetOptionPublic<bool>("async");
        Assert.True(asyncOption);
    }

    [Fact]
    public void ConstructorWithType_SetsLookupTypeAndAsyncOption()
    {
        var lookupType = typeof(TestClass);
        var attribute = new TestCustomEditorAttribute(lookupType);
        var asyncOption = attribute.GetOptionPublic<bool>("async");
        Assert.Equal(lookupType.FullName, attribute.LookupKey);
        Assert.True(asyncOption);
    }

    [Fact]
    public void SetOptionAsync_True_SetsAsyncOptionWithConstructorType()
    {
        var attribute = new TestCustomEditorAttribute(typeof(string));
        attribute.SetOptionPublic("async", true);
        Assert.True(attribute.GetOptionPublic<bool>("async"));
    }
}
