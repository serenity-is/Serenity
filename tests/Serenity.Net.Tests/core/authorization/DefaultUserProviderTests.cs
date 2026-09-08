using System.Security.Principal;

namespace Serenity.Services;

public class DefaultUserProviderTests
{
    private static ClaimsPrincipal NewPrincipal(string username)
    {
        return new ClaimsPrincipal(new GenericIdentity(username, "Test"));
    }

    private static DefaultUserProvider NewProvider(ClaimsPrincipal? user = null,
        IUserRetrieveService? retriever = null, IUserClaimCreator? claimCreator = null,
        ITwoLevelCache? cache = null)
    {
        return new DefaultUserProvider(
            new MockUserAccessor(() => user),
            claimCreator ?? new MockUserClaimCreator(),
            retriever ?? new MockUserRetrieveService(),
            cache);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenUserAccessorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultUserProvider(null, new MockUserClaimCreator(), new MockUserRetrieveService()));
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenUserClaimCreatorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultUserProvider(new MockUserAccessor(() => (ClaimsPrincipal)null), null, new MockUserRetrieveService()));
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenUserRetrieverIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultUserProvider(new MockUserAccessor(() => (ClaimsPrincipal)null), new MockUserClaimCreator(), null));
    }

    [Fact]
    public void ById_DelegatesToUserRetriever()
    {
        var user = new MockUserDefinition("42", "admin");
        var provider = NewProvider(retriever: new MockUserRetrieveService(user));

        Assert.Same(user, provider.ById("42"));
        Assert.Null(provider.ById("missing"));
    }

    [Fact]
    public void ByUsername_DelegatesToUserRetriever()
    {
        var user = new MockUserDefinition("42", "admin");
        var provider = NewProvider(retriever: new MockUserRetrieveService(user));

        Assert.Same(user, provider.ByUsername("admin"));
        Assert.Null(provider.ByUsername("missing"));
    }

    [Fact]
    public void CreatePrincipal_DelegatesToUserClaimCreator()
    {
        var creator = new MockUserClaimCreator();
        var provider = NewProvider(claimCreator: creator);

        var principal = provider.CreatePrincipal("admin", "Test");
        Assert.Equal("admin", principal.Identity.Name);
    }

    [Fact]
    public void User_ReturnsUnderlyingAccessorUser()
    {
        var user = NewPrincipal("admin");
        var provider = NewProvider(user);
        Assert.Same(user, provider.User);
    }

    [Fact]
    public void Impersonate_ThrowsInvalidOperationException_WhenAccessorIsNotImpersonator()
    {
        var provider = NewProvider();
        Assert.Throws<InvalidOperationException>(() => provider.Impersonate(NewPrincipal("x")));
        Assert.Throws<InvalidOperationException>(() => provider.UndoImpersonate());
    }

    [Fact]
    public void Impersonate_DelegatesToImpersonatingAccessor()
    {
        var accessor = new ImpersonatingUserAccessor(
            new MockUserAccessor(() => NewPrincipal("admin")), new MockHttpContextItemsAccessor());
        var provider = new DefaultUserProvider(accessor, new MockUserClaimCreator(), new MockUserRetrieveService());

        var impersonated = NewPrincipal("temp");
        provider.Impersonate(impersonated);
        Assert.Same(impersonated, provider.User);

        provider.UndoImpersonate();
        Assert.Equal("admin", provider.User.Identity.Name);
    }

    [Fact]
    public void RemoveAll_DelegatesToRemoveAll_WhenRetrieverImplementsIt()
    {
        var retriever = new MockRemoveAllUserRetriever();
        var provider = NewProvider(retriever: retriever);

        provider.RemoveAll();
        Assert.True(retriever.RemoveAllCalled);
    }

    [Fact]
    public void RemoveAll_ExpiresDefaultUsersGroup_WhenRetrieverDoesNotImplementRemoveAll()
    {
        var cache = new NullTwoLevelCache();
        var provider = NewProvider(retriever: new MockUserRetrieveService(), cache: cache);

        // Should not throw when falling back to cache group expiry.
        provider.RemoveAll();
    }

    [Fact]
    public void RemoveCachedUser_DelegatesToRetriever()
    {
        var retriever = new MockRemoveCachedUserRetriever();
        var provider = NewProvider(retriever: retriever);

        provider.RemoveCachedUser("42", "admin");
        Assert.Equal("42", retriever.UserId);
        Assert.Equal("admin", retriever.Username);
    }

    private class MockUserClaimCreator : IUserClaimCreator
    {
        public ClaimsPrincipal CreatePrincipal(string username, string authType)
        {
            return new ClaimsPrincipal(new GenericIdentity(username, authType));
        }
    }

    private class MockRemoveAllUserRetriever : IUserRetrieveService, IRemoveAll
    {
        public bool RemoveAllCalled { get; private set; }

        public IUserDefinition ById(string id) => null;
        public IUserDefinition ByUsername(string username) => null;
        public void RemoveAll() => RemoveAllCalled = true;
    }

    private class MockRemoveCachedUserRetriever : IUserRetrieveService, IRemoveCachedUser
    {
        public string UserId { get; private set; }
        public string Username { get; private set; }

        public IUserDefinition ById(string id) => null;
        public IUserDefinition ByUsername(string username) => null;
        public void RemoveCachedUser(string userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }
}
