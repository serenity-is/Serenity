using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    /// <summary>
    /// An interface to query list of default handlers, registered through the dependency resolver
    /// </summary>
    public interface IDefaultHandlerRegistry
    {
        IEnumerable<Type> GetTypes();
    }
}