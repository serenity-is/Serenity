namespace Serenity.ComponentModel;

public class CustomEditorAttributeTests
{
    public class MyEditorAttribute : CustomEditorAttribute
    {
        public MyEditorAttribute()
            : base("MyEditor")
        {
        }

        public string TestOption
        {
            get => GetOption<string>("testOption");
            set => SetOption("testOption", value);
        }

        public T GetOptionPublic<T>(string key)
        {
            return GetOption<T>(key);
        }

        public void SetOptionPublic(string key, object value)
        {
            SetOption(key, value);
        }
    }

    private readonly MyEditorAttribute myEditorAttribute;

    public CustomEditorAttributeTests() => myEditorAttribute = new MyEditorAttribute();

    [Fact]
    public void SetParams_ShouldThrow_ArgumentNullException_ForNull()
    {
        var attribute = new MyEditorAttribute();
        Assert.Throws<ArgumentNullException>(() => attribute.SetParams(null));
    }

    [Fact]
    public void SetParams_ShouldNotModify_FilteringParams_ForEmpty()
    {
        var attribute = new MyEditorAttribute();
        var dict = new Dictionary<string, object>();
        attribute.SetParams(dict);
        Assert.Empty(dict);
    }

    [Fact]
    public void SetParams_ShouldModify_FilteringParams_ForNonEmpty()
    {
        var attribute = new MyEditorAttribute
        {
            TestOption = "text"
        };
        var dict = new Dictionary<string, object>();
        attribute.SetParams(dict);
        var x = Assert.Single(dict);
        Assert.Equal("testOption", x.Key);
        Assert.Equal("text", x.Value);
    }

    [Fact]
    public void GetOption_OptionsIsNull_ReturnsDefault()
    {
        var result = myEditorAttribute.GetOptionPublic<string>("nonExistentKey");
        Assert.Null(result);
    }

    [Fact]
    public void GetOption_KeyDoesNotExist_ReturnsDefault()
    {
        myEditorAttribute.SetParams(new Dictionary<string, object>());
        var result = myEditorAttribute.GetOptionPublic<string>("nonExistentKey");
        Assert.Null(result);
    }

    [Fact]
    public void GetOption_KeyExists_ReturnsValue()
    {
        myEditorAttribute.SetOptionPublic("existingKey", "expectedValue");
        var result = myEditorAttribute.GetOptionPublic<string>("existingKey");
        Assert.Equal("expectedValue", result);
    }

    [Fact]
    public void GetOption_KeyExistsButNullValue_ReturnsDefault()
    {
        myEditorAttribute.SetOptionPublic("existingKey", null);
        var result = myEditorAttribute.GetOptionPublic<string>("existingKey");
        Assert.Null(result);
    }
}
