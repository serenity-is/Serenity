namespace Serenity.Web;

public class HttpContextUserAccessorTests
{
    [Fact]
    public void User_Is_Null_When_Accessor_Is_Null()
    {
        Assert.Null(new HttpContextUserAccessor().User);
    }

    [Fact]
    public void User_Is_Null_When_HttpContext_Is_Null()
    {
        var accessor = new MockHttpContextAccessor();
        Assert.Null(new HttpContextUserAccessor(accessor).User);
    }

    [Fact]
    public void User_Returns_HttpContext_User()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(authenticationType: "Test"));
        var accessor = new MockHttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        Assert.Same(principal, new HttpContextUserAccessor(accessor).User);
    }
}
