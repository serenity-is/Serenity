using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    /// <summary>
    /// An interface to query list of implicit behaviors registered through the dependency resolver
    /// </summary>
    public interface IImplicitBehaviorRegistry
    {
        IEnumerable<Type> GetTypes();
    }
}