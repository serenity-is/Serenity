using System;

namespace Serenity.Abstractions
{
    public class DateTimeProvider : IDateTimeProvider
    {
        private static DateTimeProvider instance = new DateTimeProvider();

        private DateTimeProvider()
        {
            StaticProvider = instance;
        }

        public static IDateTimeProvider StaticProvider;

        public static IDateTimeProvider Current
        {
            get
            {
                return StaticProvider ?? Dependency.TryResolve<IDateTimeProvider>() ?? instance;
            }
        }

        public static DateTime Now
        {
            get { return Current.Now; }
        }

        DateTime IDateTimeProvider.Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}