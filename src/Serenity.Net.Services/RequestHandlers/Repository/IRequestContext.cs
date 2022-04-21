namespace Serenity.Services
{
    public interface IRequestContext
    {
        IBehaviorProvider Behaviors { get; }
        ITwoLevelCache Cache { get; }
        ITextLocalizer Localizer { get; }
        IPermissionService Permissions { get; }
        ClaimsPrincipal User { get; }
    }
}