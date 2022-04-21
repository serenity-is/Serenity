namespace Serenity.Services
{
    public abstract class BaseRequestHandler : IRequestHandler
    {
        public BaseRequestHandler(IRequestContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected ITwoLevelCache Cache => Context.Cache;
        protected IRequestContext Context { get; }
        protected ITextLocalizer Localizer => Context.Localizer;
        protected IPermissionService Permissions => Context.Permissions;
        protected ClaimsPrincipal User => Context.User;
    }
}