using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    /// IRowTypeRegistry
    /// </summary>
    public interface IRowTypeRegistry
    {
        /// <summary>
        /// Gets all row types.
        /// </summary>
        /// <value>
        /// All row types.
        /// </value>
        IEnumerable<Type> AllRowTypes { get; }
        /// <summary>
        /// Bies the connection key.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns></returns>
        IEnumerable<Type> ByConnectionKey(string connectionKey);
    }
}