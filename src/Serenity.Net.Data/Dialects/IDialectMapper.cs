using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    /// The dialect mapper
    /// </summary>
    public interface IDialectMapper
    {
        /// <summary>
        /// Returns dialect for a dialect or provider name
        /// </summary>
        /// <param name="dialectOrProviderName">The dialect name or provider name</param>
        ISqlDialect TryGet(string dialectOrProviderName);
    }
}
