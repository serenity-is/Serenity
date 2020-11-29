using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Type source extension methods
    /// </summary>
    public static class TypeSourceExtensions
    {
        /// <summary>
        /// Gets all attributes for assemblies
        /// </summary>
        /// <returns>List of attributes for assemblies</returns>
        public static IEnumerable<TAttribute> GetAssemblyAttributes<TAttribute>(this ITypeSource typeSource)
            where TAttribute: Attribute
        {
            return typeSource.GetAssemblyAttributes(typeof(TAttribute)).Cast<TAttribute>();
        }
    }
}