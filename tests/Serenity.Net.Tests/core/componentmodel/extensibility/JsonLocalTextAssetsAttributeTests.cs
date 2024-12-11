namespace Serenity.ComponentModel;

public class JsonLocalTextAssetsAttributeTests()
{
    [Fact]
    public void Constructor_ShouldThrow_ArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => new JsonLocalTextAssetsAttribute(null));
    }

    [Fact]
    public void Path_CanBePassed_AsString()
    {
        var attribute = new JsonLocalTextAssetsAttribute("path");
        Assert.Equal("path", attribute.Path);
    }
}