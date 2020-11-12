#if NET45
using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Date time abstraction for unit tests.
    /// </summary>
    /// <seealso cref="Serenity.Abstractions.IDateTimeProvider" />
    public class DateTimeProvider : IDateTimeProvider
    {
        private static DateTimeProvider instance = new DateTimeProvider();

        private DateTimeProvider()
        {
            StaticProvider = instance;
        }

        /// <summary>
        /// The static provider
        /// </summary>
        public static IDateTimeProvider StaticProvider;

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        /// <value>
        /// The current provider.
        /// </value>
        public static IDateTimeProvider Current
        {
            get
            {
                return StaticProvider ?? Dependency.TryResolve<IDateTimeProvider>() ?? instance;
            }
        }

        /// <summary>
        /// Gets the date/time now.
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        public static DateTime Now
        {
            get { return Current.Now; }
        }

        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        DateTime IDateTimeProvider.Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
#endif