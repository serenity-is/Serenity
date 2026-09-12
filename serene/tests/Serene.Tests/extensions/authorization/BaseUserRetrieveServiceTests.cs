namespace Serenity.Extensions;

public class BaseUserRetrieveServiceTests
{
    private class TestUserRetrieveService(ITwoLevelCache cache) : BaseUserRetrieveService(cache)
    {
        public Func<string, IUserDefinition?> LoadByIdFunc { get; set; } = _ => null;
        public Func<string, IUserDefinition?> LoadByUsernameFunc { get; set; } = _ => null;
        public int LoadByIdCount { get; private set; }
        public int LoadByUsernameCount { get; private set; }

        protected override string GetCacheGroupKey() => "TestUserGroup";
        protected override IUserDefinition? LoadById(string id)
        {
            LoadByIdCount++;
            return LoadByIdFunc(id);
        }

        protected override IUserDefinition? LoadByUsername(string username)
        {
            LoadByUsernameCount++;
            return LoadByUsernameFunc(username);
        }

        public string IdCacheKey(string id) => GetIdCacheKey(id);
        public string UsernameCacheKey(string username) => GetUsernameCacheKey(username);
        public TimeSpan CacheDuration() => GetCacheDuration();
    }

    [Fact]
    public void Constructor_Throws_For_Null_Cache()
    {
        Assert.Throws<ArgumentNullException>(() => new TestUserRetrieveService(null!));
    }

    [Fact]
    public void ById_Returns_Null_For_Invalid_Id()
    {
        var service = new TestUserRetrieveService(new TestTwoLevelCache());
        Assert.Null(service.ById(""));
        Assert.Null(service.ById(null!));
    }

    [Fact]
    public void ByUsername_Returns_Null_For_Invalid_Username()
    {
        var service = new TestUserRetrieveService(new TestTwoLevelCache());
        Assert.Null(service.ByUsername(""));
        Assert.Null(service.ByUsername(null!));
    }

    [Fact]
    public void ById_Loads_And_Caches()
    {
        var service = new TestUserRetrieveService(new TestTwoLevelCache());
        var user = new MockUserDefinition("1", "user1");
        service.LoadByIdFunc = id => id == "1" ? user : null;

        Assert.Same(user, service.ById("1"));
        Assert.Same(user, service.ById("1"));
        Assert.Equal(1, service.LoadByIdCount);
    }

    [Fact]
    public void ByUsername_Loads_And_Caches_CaseInsensitively()
    {
        var service = new TestUserRetrieveService(new TestTwoLevelCache());
        var user = new MockUserDefinition("1", "user1");
        service.LoadByUsernameFunc = username => username.Equals("user1", StringComparison.OrdinalIgnoreCase) ? user : null;

        Assert.Same(user, service.ByUsername("User1"));
        Assert.Same(user, service.ByUsername("user1"));
        Assert.Equal(1, service.LoadByUsernameCount);
    }

    [Fact]
    public void Cache_Keys_And_Duration_Are_Expected()
    {
        var service = new TestUserRetrieveService(new TestTwoLevelCache());
        Assert.Equal("UserByID_1", service.IdCacheKey("1"));
        Assert.Equal("UserByName_user1", service.UsernameCacheKey("User1"));
        Assert.Equal(TimeSpan.Zero, service.CacheDuration());
    }

    [Fact]
    public void RemoveAll_Expires_Cached_Users()
    {
        var service = new TestUserRetrieveService(new TestTwoLevelCache());
        var user = new MockUserDefinition("1", "user1");
        service.LoadByIdFunc = _ => user;

        Assert.Same(user, service.ById("1"));
        service.RemoveAll();
        Assert.Same(user, service.ById("1"));
        Assert.Equal(2, service.LoadByIdCount);
    }

    [Fact]
    public void RemoveCachedUser_Removes_Both_Keys()
    {
        var service = new TestUserRetrieveService(new TestTwoLevelCache());
        var user = new MockUserDefinition("1", "user1");
        service.LoadByIdFunc = _ => user;
        service.LoadByUsernameFunc = _ => user;

        service.ById("1");
        service.ByUsername("user1");
        service.RemoveCachedUser("1", "user1");

        service.ById("1");
        service.ByUsername("user1");
        Assert.Equal(2, service.LoadByIdCount);
        Assert.Equal(2, service.LoadByUsernameCount);
    }

    [Fact]
    public void RemoveCachedUser_Ignores_Invalid_Keys()
    {
        var cache = new TestTwoLevelCache();
        var service = new TestUserRetrieveService(cache);
        var user = new MockUserDefinition("1", "user1");
        service.LoadByIdFunc = _ => user;
        service.LoadByUsernameFunc = _ => user;

        service.ById("1");
        service.ByUsername("user1");
        service.RemoveCachedUser(null, null);
        service.RemoveCachedUser("", "");

        Assert.True(cache.Memory.TryGetValue("UserByID_1", out _));
        Assert.True(cache.Memory.TryGetValue("UserByName_user1", out _));
        Assert.Same(user, service.ById("1"));
        Assert.Same(user, service.ByUsername("user1"));
    }
}
