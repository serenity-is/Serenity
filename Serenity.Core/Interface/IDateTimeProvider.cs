#if NET45
using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Date/time abstraction for unit testing.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        DateTime Now { get; }
    }
}
#endif