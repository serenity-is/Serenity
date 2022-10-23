namespace Serenity.Tests;

public class MockRequestContext : IRequestContext
{
    public IBehaviorProvider Behaviors { get; }
    public ITwoLevelCache Cache { get; }
    public ITextLocalizer Localizer { get; }
    public IPermissionService Permissions { get; }
    public ClaimsPrincipal User { get; }
    
    
    public MockRequestContext(IBehaviorProvider behaviors = null,
        ITwoLevelCache cache = null,
        ITextLocalizer localizer = null,
        IPermissionService permissions = null,
        ClaimsPrincipal user = null)
    {
        Behaviors = behaviors;
        Cache = cache;
        Localizer = localizer;
        Permissions = permissions;
        User = user;
    }
}