using Serenity.Abstractions;
using System;
using System.Collections.Generic;

namespace Serenity.Abstractions
{
    public interface IServiceContext
    {
        IEnumerable<TBehavior> GetBehaviors<TBehavior>(Type entityType);
        IUserContext UserContext { get; }
        ILocalTextContext Localization { get; }
    }
}