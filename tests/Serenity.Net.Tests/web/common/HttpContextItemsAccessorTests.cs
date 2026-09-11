namespace Serenity.Web;

public class HttpContextItemsAccessorTests
{
    [Fact]
    public void Items_Is_Null_When_Accessor_Is_Null()
    {
        Assert.Null(new HttpContextItemsAccessor().Items);
    }

    [Fact]
    public void Items_Is_Null_When_HttpContext_Is_Null()
    {
        var accessor = new MockHttpContextAccessor();
        Assert.Null(new HttpContextItemsAccessor(accessor).Items);
    }

    [Fact]
    public void Items_Returns_HttpContext_Items()
    {
        var context = new DefaultHttpContext();
        var key = new object();
        context.Items[key] = "value";
        var accessor = new MockHttpContextAccessor { HttpContext = context };

        var items = new HttpContextItemsAccessor(accessor).Items;

        Assert.NotNull(items);
        Assert.Equal("value", items[key]);
    }
}
