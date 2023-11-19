namespace Serenity.Tests;

public class NullRequestContext(IBehaviorProvider behaviors = null,
    ITwoLevelCache cache = null,
    ITextLocalizer localizer = null,
    IPermissionService permissions = null,
    IUserAccessor userAccessor = null) : IRequestContext
{
    public IBehaviorProvider Behaviors { get; set; } = behaviors ?? new NullBehaviorProvider();
    public ITwoLevelCache Cache { get; set; } = cache ?? new NullTwoLevelCache();
    public ITextLocalizer Localizer { get; set; } = localizer ?? NullTextLocalizer.Instance;
    public IPermissionService Permissions { get; set; } = permissions ?? new NullPermissions();
    public IUserAccessor UserAccessor { get; set; } = userAccessor ?? new NullUserAccessor();
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