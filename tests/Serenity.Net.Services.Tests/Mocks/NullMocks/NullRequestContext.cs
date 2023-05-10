namespace Serenity.Tests;

public class NullRequestContext : IRequestContext
{
    public NullRequestContext(IBehaviorProvider behaviors = null,
        ITwoLevelCache cache = null,
        ITextLocalizer localizer = null,
        IPermissionService permissions = null,
        IUserAccessor userAccessor = null)
    {
        Behaviors = behaviors ?? new NullBehaviorProvider();
        Cache = cache ?? new NullTwoLevelCache();
        Localizer = localizer ?? NullTextLocalizer.Instance;
        Permissions = permissions ?? new NullPermissions();
        UserAccessor = userAccessor ?? new NullUserAccessor();
    }

    public IBehaviorProvider Behaviors { get; set; }
    public ITwoLevelCache Cache { get; set; }
    public ITextLocalizer Localizer { get; set; }
    public IPermissionService Permissions { get; set; }
    public IUserAccessor UserAccessor { get; set; }
    public ClaimsPrincipal User => UserAccessor?.User;

    public NullRequestContext AsSysAdmin()
    {
        UserAccessor = new MockUserAccessor(() => TestUser.SysAdmin);
        Permissions = new MockPermissions(perm => true);
        return this;
    }

    public NullRequestContext AsGuest()
    {
        UserAccessor = new MockUserAccessor(() => TestUser.Guest);
        Permissions = new MockPermissions(perm => false);
        return this;
    }

    public NullRequestContext WithPermissions(Func<string, bool> hasPermission)
    {
        Permissions = new MockPermissions(hasPermission);
        return this;
    }
}