using System.Security.Principal;

namespace Serenity.Tests.Authorization;

public class AuthorizationExtensionsTests
{
    [Fact]
    public void IsLoggedIn_Returns_False_If_UserAccessor_IsNull()
    {
        Assert.False(((IUserAccessor)null).IsLoggedIn());
    }


    [Fact]
    public void IsLoggedIn_Returns_False_If_UserAccessor_User_Property_IsNull()
    {
        var userAccessor = new MockUserAccessor(() => (ClaimsPrincipal)null);
        Assert.False(userAccessor.IsLoggedIn());
    }

    [Fact]
    public void IsLoggedIn_Returns_False_If_UserAccessor_User_Identity_IsNull()
    {
        var userAccessor = new MockUserAccessor(() => new ClaimsPrincipal());
        Assert.False(userAccessor.IsLoggedIn());
    }

    [Fact]
    public void IsLoggedIn_Returns_False_If_UserAccessor_User_Identity_IsAuthenticated_IsFalse()
    {
        var identity = new ClaimsIdentity();
        Assert.False(identity.IsAuthenticated);
        var userAccessor = new MockUserAccessor(() => new ClaimsPrincipal(identity));
        Assert.False(userAccessor.IsLoggedIn());
    }

    [Fact]
    public void IsLoggedIn_Returns_True_If_UserAccessor_User_Identity_IsAuthenticated_IsTrue()
    {
        var identity = new GenericIdentity("test");
        Assert.True(identity.IsAuthenticated);
        var userAccessor = new MockUserAccessor(() => new ClaimsPrincipal(identity));
        Assert.True(userAccessor.IsLoggedIn());
    }

    [Fact]
    public void IsLoggedIn_Returns_False_If_Principal_IsNull()
    {
        Assert.False(((ClaimsPrincipal)null).IsLoggedIn());
    }

    [Fact]
    public void IsLoggedIn_Returns_False_If_Principal_Identity_IsNull()
    {
        var principal = new ClaimsPrincipal();
        Assert.False(principal.IsLoggedIn());
    }

    [Fact]
    public void IsLoggedIn_Returns_False_If_Principal_User_Identity_IsAuthenticated_IsFalse()
    {
        var identity = new ClaimsIdentity();
        Assert.False(identity.IsAuthenticated);
        Assert.False(new ClaimsPrincipal(identity).IsLoggedIn());
    }

    [Fact]
    public void IsLoggedIn_Returns_True_If_Principal_User_Identity_IsAuthenticated_IsTrue()
    {
        var identity = new GenericIdentity("test");
        Assert.True(identity.IsAuthenticated);
        Assert.True(new ClaimsPrincipal(identity).IsLoggedIn());
    }
}