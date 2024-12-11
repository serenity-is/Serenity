namespace Serenity.ComponentModel;

public class CustomFilteringAttributeTests
{
    class MyFilteringAttribute : CustomFilteringAttribute
    {
        public MyFilteringAttribute()
            : base("text")
        {
        }

        public string TestOption
        {
            get => GetOption<string>("testOption");
            set => SetOption("testOption", value);
        }
    }

    [Fact]
    public void SetParams_ShouldThrow_ArgumentNullException_ForNull()
    {
        var attribute = new MyFilteringAttribute();
        Assert.Throws<ArgumentNullException>(() => attribute.SetParams(null));
    }

    [Fact]
    public void SetParams_ShouldNotModify_FilteringParams_ForEmpty()
    {
        var attr = new MyFilteringAttribute();
        var dict = new Dictionary<string, object>();
        attr.SetParams(dict);
        Assert.Empty(dict);
    }

    [Fact]
    public void SetParams_ShouldModify_FilteringParams_ForNonEmpty()
    {
        var attr = new MyFilteringAttribute
        {
            TestOption = "myvalue"
        };
        var dict = new Dictionary<string, object>();
        attr.SetParams(dict);
        var x = Assert.Single(dict);
        Assert.Equal("testOption", x.Key);
        Assert.Equal("myvalue", x.Value);
    }
}



