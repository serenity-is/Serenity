namespace Serenity.ComponentModel;

public class CustomFormatterAttributeTests
{
    class MyFormatter : CustomFormatterAttribute
    {
        public MyFormatter()
            : base("test")
        {
        }

        public string TestOption
        {
            get => GetOption<string>("testOption");
            set => SetOption("testOption", value);
        }
    }

    [Fact]
    public void SetParams_ShouldThrowArgumentNullException_ForNull()
    {
        var attribute = new MyFormatter();
        Assert.Throws<ArgumentNullException>(() => attribute.SetParams(null));
    }

    [Fact]
    public void SetParams_ShouldNotModify_FormatterParams_ForEmpty()
    {
        var attr = new MyFormatter();
        var dict = new Dictionary<string, object>();
        attr.SetParams(dict);
        Assert.Empty(dict);
    }

    [Fact]
    public void SetParams_ShouldModify_FormatterParams_ForNonEmpty()
    {
        var attr = new MyFormatter
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