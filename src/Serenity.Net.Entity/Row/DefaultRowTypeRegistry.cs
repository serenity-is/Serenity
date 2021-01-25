using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Data
{
    /// <summary>
    /// Default row type registry
    /// </summary>
    /// <seealso cref="IRowTypeRegistry" />
    public class DefaultRowTypeRegistry : IRowTypeRegistry
    {
        private readonly IEnumerable<Type> rowTypes;
        private readonly ILookup<string, Type> byConnectionKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRowTypeRegistry"/> class.
        /// </summary>
        /// <param name="typeSource">The type source.</param>
        /// <exception cref="ArgumentNullException">typeSource</exception>
        public DefaultRowTypeRegistry(ITypeSource typeSource)
        {
            rowTypes = (typeSource ?? throw new ArgumentNullException(nameof(typeSource)))
                .GetTypesWithInterface(typeof(IRow))
                .Where(x => !x.IsAbstract && !x.IsInterface);

            byConnectionKey = rowTypes.Where(x => x.GetCustomAttribute<ConnectionKeyAttribute>() != null)
                .ToLookup(x => x.GetCustomAttribute<ConnectionKeyAttribute>().Value);
        }

        /// <summary>
        /// Gets all row types.
        /// </summary>
        /// <value>
        /// All row types.
        /// </value>
        public IEnumerable<Type> AllRowTypes => rowTypes;

        /// <summary>
        /// Bies the connection key.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns></returns>
        public IEnumerable<Type> ByConnectionKey(string connectionKey)
        {
            return byConnectionKey[connectionKey];
        }
    }
}