using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Principal;

namespace Serenity;

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

    [Fact]
    public void ValidatePermission_Throws_ArgumentNullException_If_Permissions_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IPermissionService)null).ValidatePermission("A", NullTextLocalizer.Instance));
    }

    [Fact]
    public void ValidatePermission_Throws_ValidationError_If_NotGranted()
    {
        var permissions = new MockPermissions(p => false);
        var ex = Assert.Throws<ValidationError>(() =>
            permissions.ValidatePermission("A", NullTextLocalizer.Instance));
        Assert.Equal("AccessDenied", ex.ErrorCode);
    }

    [Fact]
    public void ValidatePermission_DoesNotThrow_If_Granted()
    {
        var permissions = new MockPermissions(p => true);
        permissions.ValidatePermission("A", NullTextLocalizer.Instance);
    }

    [Fact]
    public void ValidateLoggedIn_Throws_ValidationError_If_NotLoggedIn()
    {
        var userAccessor = new MockUserAccessor(() => (ClaimsPrincipal)null);
        var ex = Assert.Throws<ValidationError>(() =>
            userAccessor.ValidateLoggedIn(NullTextLocalizer.Instance));
        Assert.Equal("NotLoggedIn", ex.ErrorCode);
    }

    [Fact]
    public void ValidateLoggedIn_DoesNotThrow_If_LoggedIn()
    {
        var userAccessor = new MockUserAccessor(() => new ClaimsPrincipal(new GenericIdentity("test")));
        userAccessor.ValidateLoggedIn(NullTextLocalizer.Instance);
    }

    [Fact]
    public void GetIdentifier_Returns_Null_If_Principal_IsNull()
    {
        Assert.Null(((ClaimsPrincipal)null).GetIdentifier());
    }

    [Fact]
    public void GetIdentifier_Returns_Null_If_No_NameIdentifier_Claim()
    {
        var principal = new ClaimsPrincipal(new GenericIdentity("test"));
        Assert.Null(principal.GetIdentifier());
    }

    [Fact]
    public void GetIdentifier_Returns_NameIdentifier_Claim_Value()
    {
        var identity = new GenericIdentity("test");
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "42"));
        Assert.Equal("42", new ClaimsPrincipal(identity).GetIdentifier());
    }

    [Fact]
    public void GetUserDefinition_Returns_Null_If_NotLoggedIn()
    {
        var principal = new ClaimsPrincipal();
        var retriever = new MockUserRetrieveService(new MockUserDefinition("42", "test"));
        Assert.Null(principal.GetUserDefinition(retriever));
        Assert.Null(principal.GetUserDefinition<MockUserDefinition>(retriever));
    }

    [Fact]
    public void GetUserDefinition_Returns_User_By_Username()
    {
        var user = new MockUserDefinition("42", "test");
        var retriever = new MockUserRetrieveService(user);
        var principal = new ClaimsPrincipal(new GenericIdentity("test"));

        Assert.Same(user, principal.GetUserDefinition(retriever));
        Assert.Same(user, principal.GetUserDefinition<MockUserDefinition>(retriever));
    }

    [Fact]
    public void GetUserDefinition_Returns_Null_If_User_Not_Found()
    {
        var retriever = new MockUserRetrieveService();
        var principal = new ClaimsPrincipal(new GenericIdentity("missing"));
        Assert.Null(principal.GetUserDefinition(retriever));
    }

    [Fact]
    public void GetUserDefinition_On_Retriever_Returns_User_By_Username()
    {
        var user = new MockUserDefinition("42", "test");
        var retriever = new MockUserRetrieveService(user);
        var principal = new ClaimsPrincipal(new GenericIdentity("test"));

        Assert.Same(user, retriever.GetUserDefinition(principal));
    }

    [Fact]
    public void GetUserDefinition_On_UserProvider_Returns_Null_If_NotLoggedIn()
    {
        var provider = new MockUserProvider(new MockUserAccessor(() => (ClaimsPrincipal)null));
        Assert.Null(provider.GetUserDefinition());
    }

    [Fact]
    public void GetUserDefinition_On_UserProvider_Returns_User()
    {
        var user = new MockUserDefinition("42", "test");
        var provider = new MockUserProvider(
            new MockUserAccessor(() => new ClaimsPrincipal(new GenericIdentity("test"))),
            new MockUserRetrieveService(user));
        Assert.Same(user, provider.GetUserDefinition());
    }

    [Fact]
    public void Impersonate_Delegates_To_UserProvider()
    {
        var accessor = new ImpersonatingUserAccessor(
            new MockUserAccessor(() => new ClaimsPrincipal(new GenericIdentity("admin"))),
            new MockHttpContextItemsAccessor());
        var provider = new DefaultUserProvider(accessor, new MockUserClaimCreator(), new MockUserRetrieveService());

        provider.Impersonate("temp");
        Assert.Equal("temp", provider.User.Identity.Name);
    }

    [Fact]
    public void RemoveCachedUser_Delegates_To_RemoveCachedUser_When_Supported()
    {
        var retriever = new MockRemoveCachedUserRetriever();
        var user = new MockUserDefinition("42", "test");
        retriever.RemoveCachedUser(user, null);
        Assert.Equal("42", retriever.UserId);
        Assert.Equal("test", retriever.Username);
    }

    [Fact]
    public void RemoveCachedUser_Removes_From_Cache_When_Not_Supported()
    {
        var retriever = new MockUserRetrieveService();
        var cache = new MockTwoLevelCache();
        var user = new MockUserDefinition("42", "test");

        retriever.RemoveCachedUser(user, cache);
        var removed = ((TrackingMemoryCache)cache.Memory).RemovedKeys;
        Assert.Contains("UserById_42", removed);
        Assert.Contains("UserByName_test", removed);
    }

    [Fact]
    public void RemoveCachedUser_By_Id_And_Username_Removes_From_Cache()
    {
        var retriever = new MockUserRetrieveService();
        var cache = new MockTwoLevelCache();

        retriever.RemoveCachedUser("42", "Test", cache);
        var removed = ((TrackingMemoryCache)cache.Memory).RemovedKeys;
        Assert.Contains("UserById_42", removed);
        Assert.Contains("UserByName_test", removed);
    }

    private class MockUserClaimCreator : IUserClaimCreator
    {
        public ClaimsPrincipal CreatePrincipal(string username, string authType)
        {
            return new ClaimsPrincipal(new GenericIdentity(username, authType));
        }
    }

    private class MockUserProvider : IUserProvider
    {
        private readonly IUserAccessor accessor;
        private readonly IUserRetrieveService retriever;
        private readonly IUserClaimCreator claimCreator;

        public MockUserProvider(IUserAccessor accessor, IUserRetrieveService? retriever = null,
            IUserClaimCreator? claimCreator = null)
        {
            this.accessor = accessor;
            this.retriever = retriever ?? new MockUserRetrieveService();
            this.claimCreator = claimCreator ?? new MockUserClaimCreator();
        }

        public ClaimsPrincipal User => accessor.User;
        public IUserDefinition ById(string id) => retriever.ById(id);
        public IUserDefinition ByUsername(string username) => retriever.ByUsername(username);
        public ClaimsPrincipal CreatePrincipal(string username, string authType) => claimCreator.CreatePrincipal(username, authType);
        public void Impersonate(ClaimsPrincipal user) { }
        public void UndoImpersonate() { }
        public void RemoveAll() { }
        public void RemoveCachedUser(string userId, string username) { }
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

    private class MockTwoLevelCache : ITwoLevelCache
    {
        public List<string> RemovedKeys { get; } = [];

        public IMemoryCache Memory { get; } = new TrackingMemoryCache();
        public IDistributedCache Distributed { get; } = new NullDistributedCache();
    }

    private class TrackingMemoryCache : IMemoryCache
    {
        private readonly Dictionary<object, object> items = [];
        public List<string> RemovedKeys { get; } = [];

        public ICacheEntry CreateEntry(object key)
        {
            var entry = new MockMemoryCache.CacheEntry { Key = key };
            items[key] = entry;
            return entry;
        }

        public void Dispose() { }

        public void Remove(object key)
        {
            items.Remove(key);
            if (key is string s)
                RemovedKeys.Add(s);
        }

        public bool TryGetValue(object key, out object value)
        {
            if (items.TryGetValue(key, out var v))
            {
                value = v;
                return true;
            }
            value = null;
            return false;
        }
    }
}