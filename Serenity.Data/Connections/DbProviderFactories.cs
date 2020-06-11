#if !NET45
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Serenity.Data
{
    /// <summary>
    /// Registry and extension methods for DB provider factories
    /// </summary>
    public static class DbProviderFactories
    {
        internal static readonly Dictionary<string, Func<DbProviderFactory>> _configs = new Dictionary<string, Func<DbProviderFactory>>();

        /// <summary>
        /// Gets a factory by its invariant name.
        /// </summary>
        /// <param name="providerInvariantName">Name of the provider.</param>
        /// <returns>Provider with given name</returns>
        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            if (_configs.ContainsKey(providerInvariantName))
                return _configs[providerInvariantName]();

            throw new ArgumentOutOfRangeException("providerInvariantName");
        }

        /// <summary>
        /// Registers a factory with a provider name.
        /// </summary>
        /// <param name="providerInvariantName">Name of the provider.</param>
        /// <param name="factory">The factory.</param>
        public static void RegisterFactory(string providerInvariantName, DbProviderFactory factory)
        {
            _configs[providerInvariantName] = () => factory;
        }
    }
}
#endif