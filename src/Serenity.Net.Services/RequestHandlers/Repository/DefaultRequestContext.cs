namespace Serenity.Services
{
    public class DefaultRequestContext : IRequestContext
    {
        private readonly IUserAccessor userAccessor;

        public DefaultRequestContext(IBehaviorProvider behaviors, ITwoLevelCache cache, ITextLocalizer localizer, 
            IPermissionService permissions, IUserAccessor userAccessor)
        {
            Behaviors = behaviors ?? throw new ArgumentNullException(nameof(behaviors));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        }

        public IBehaviorProvider Behaviors { get; private set; }
        public ITwoLevelCache Cache { get; private set; }
        public ITextLocalizer Localizer { get; private set; }
        public IPermissionService Permissions { get; private set; }
        public ClaimsPrincipal User => userAccessor.User;
    }
}

