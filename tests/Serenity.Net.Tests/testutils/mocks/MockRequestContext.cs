namespace Serenity.Tests;

public class MockRequestContext(IBehaviorProvider behaviors = null,
    ITwoLevelCache cache = null,
    ITextLocalizer localizer = null,
    IPermissionService permissions = null,
    ClaimsPrincipal user = null) : IRequestContext
{
    public IBehaviorProvider Behaviors { get; } = behaviors;
    public ITwoLevelCache Cache { get; } = cache;
    public ITextLocalizer Localizer { get; } = localizer;
    public IPermissionService Permissions { get; } = permissions;
    public ClaimsPrincipal User { get; } = user;
}