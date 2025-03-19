namespace Serenity.TestUtils;

public class MockRequestContext(IBehaviorProvider behaviors = null,
    ITwoLevelCache cache = null,
    ITextLocalizer localizer = null,
    IPermissionService permissions = null,
    ClaimsPrincipal user = null) : IRequestContext
{
    public IBehaviorProvider Behaviors { get; set; } = behaviors;
    public ITwoLevelCache Cache { get; set; } = cache;
    public ITextLocalizer Localizer { get; set; } = localizer;
    public IPermissionService Permissions { get; set; } = permissions;
    public ClaimsPrincipal User { get; set; } = user;
}