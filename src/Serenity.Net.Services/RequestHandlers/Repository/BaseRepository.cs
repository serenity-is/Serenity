using Serenity.Abstractions;
using System;
using System.Security.Claims;

namespace Serenity.Services
{
    public class BaseRepository
    {
        public BaseRepository(IRequestContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected IRequestContext Context { get; }
        protected ITextLocalizer Localizer => Context.Localizer;
        protected IPermissionService Permissions => Context.Permissions;
        protected ClaimsPrincipal User => Context.User;
    }
}