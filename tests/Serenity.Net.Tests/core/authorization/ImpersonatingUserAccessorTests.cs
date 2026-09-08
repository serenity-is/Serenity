using System.Security.Principal;

namespace Serenity.Web;

public class ImpersonatingUserAccessorTests
{
    private static ClaimsPrincipal NewPrincipal(string username)
    {
        return new ClaimsPrincipal(new GenericIdentity(username, "Test"));
    }

    private static ImpersonatingUserAccessor NewAccessor(ClaimsPrincipal? user = null,
        MockHttpContextItemsAccessor? items = null)
    {
        return new ImpersonatingUserAccessor(new MockUserAccessor(() => user), items);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenUserContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ImpersonatingUserAccessor(null!, new MockHttpContextItemsAccessor()));
    }

    [Fact]
    public void User_ReturnsUnderlyingUser_WhenNoImpersonationIsActive()
    {
        var underlying = NewPrincipal("admin");
        var accessor = NewAccessor(underlying);
        Assert.Same(underlying, accessor.User);
    }

    [Fact]
    public void User_ReturnsNull_WhenUnderlyingUserIsNull()
    {
        var accessor = NewAccessor(null);
        Assert.Null(accessor.User);
    }

    [Fact]
    public void Impersonate_ThenUndo_RestoresUnderlyingUser()
    {
        var underlying = NewPrincipal("admin");
        var accessor = NewAccessor(underlying);

        var impersonated = NewPrincipal("impersonated");
        accessor.Impersonate(impersonated);
        Assert.Same(impersonated, accessor.User);

        accessor.UndoImpersonate();
        Assert.Same(underlying, accessor.User);
    }

    [Fact]
    public void NestedImpersonations_ArePoppedInReverseOrder()
    {
        var underlying = NewPrincipal("admin");
        var accessor = NewAccessor(underlying);

        var first = NewPrincipal("first");
        var second = NewPrincipal("second");
        accessor.Impersonate(first);
        accessor.Impersonate(second);
        Assert.Same(second, accessor.User);

        accessor.UndoImpersonate();
        Assert.Same(first, accessor.User);

        accessor.UndoImpersonate();
        Assert.Same(underlying, accessor.User);
    }

    [Fact]
    public void Impersonate_ThrowsArgumentNullException_WhenUserIsNull()
    {
        var accessor = NewAccessor(NewPrincipal("admin"));
        Assert.Throws<ArgumentNullException>(() => accessor.Impersonate(null));
    }

    [Fact]
    public void UndoImpersonate_ThrowsInvalidOperationException_WhenStackIsEmpty()
    {
        var accessor = NewAccessor(NewPrincipal("admin"));
        Assert.Throws<InvalidOperationException>(() => accessor.UndoImpersonate());
    }

    [Fact]
    public void UsesRequestContextItems_WhenItemsAccessorIsProvided()
    {
        var items = new MockHttpContextItemsAccessor();
        var underlying = NewPrincipal("admin");
        var accessor = new ImpersonatingUserAccessor(new MockUserAccessor(() => underlying), items);

        var impersonated = NewPrincipal("temp");
        accessor.Impersonate(impersonated);
        Assert.Same(impersonated, accessor.User);
        Assert.True(items.Items.ContainsKey("ImpersonationStack"));

        accessor.UndoImpersonate();
        Assert.Same(underlying, accessor.User);
    }

    [Fact]
    public void UsesAsyncLocal_WhenNoItemsAccessorIsAvailable()
    {
        var underlying = NewPrincipal("admin");
        var accessor = NewAccessor(underlying, items: null);

        var impersonated = NewPrincipal("temp");
        accessor.Impersonate(impersonated);
        Assert.Same(impersonated, accessor.User);

        accessor.UndoImpersonate();
        Assert.Same(underlying, accessor.User);
    }
}
