using Microsoft.Extensions.Caching.Memory;
using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Serenity.Services
{
    public interface IRequestHandlerContext
    {
        IEnumerable<TBehavior> GetBehaviors<TBehavior>(Type entityType, Type handlerType);
        ClaimsPrincipal User { get; }
        ITextLocalizer Localizer { get; }
        IPermissionService Permissions { get; }
        ITwoLevelCache Cache { get; }
    }
}