using System;

namespace Serenity
{
    public static class CacheExpiration
    {
        public static readonly TimeSpan FifteenSeconds = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan ThirtyMinutes = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan OneHour = TimeSpan.FromMinutes(60);
        public static readonly TimeSpan OneDay = TimeSpan.FromDays(1);
        public static readonly TimeSpan Never = System.Web.Caching.Cache.NoSlidingExpiration;
    }
}
