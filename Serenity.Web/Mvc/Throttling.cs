using System;
using System.Web.Mvc;
using System.Web;
using System.Web.Caching;
using System.Net;
using System.Web.Hosting;

namespace Serenity.Web
{
    public class ThrottleLimit
    {
        public ThrottleLimit(string key, TimeSpan duration, int limit)
        {
            Key = key;
            Duration = duration;
            Limit = limit;
            CacheKey = "Throttling$" + key + "$" + duration.Ticks.ToInvariant(); 
        }

        public string Key { get; private set; }
        public TimeSpan Duration { get; private set; }
        public int Limit { get; private set; }
        public string CacheKey { get; private set; }
    }

    public static class Throttling
    {
        private class HitInfo
        {
            public int Counter;
        }

        public static bool Check(ThrottleLimit limit)
        {
            var cache = HostingEnvironment.Cache;

            var cacheKey = limit.CacheKey;
            var hit = (HitInfo)(cache[cacheKey]);
            if (hit == null)
            {
                hit = new HitInfo { Counter = 1 };
                cache.Add(cacheKey, hit, null, DateTime.Now.Add(limit.Duration),
                    Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.BelowNormal, null);
            }
            else 
            {
                if (hit.Counter++ > limit.Limit)
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Decorates any MVC route that needs to have client requests limited by time.
    /// </summary>
    /// <remarks>
    /// Uses the current System.Web.Caching.Cache to store each client request to the decorated route.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ThrottleAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// A unique name for this Throttle.
        /// </summary>
        /// <remarks>
        /// We'll be inserting a Cache record based on this name and client IP, e.g. "Name-192.168.0.1"
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// The number of seconds clients must wait before executing this decorated route again.
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// A text message that will be sent to the client upon throttling.  You can include the token {n} to
        /// show this.Seconds in the message, e.g. "Wait {n} seconds before trying again".
        /// </summary>
        public string Message { get; set; }

        public override void OnActionExecuting(ActionExecutingContext c)
        {
            var key = string.Concat(Name, "-", c.HttpContext.Request.UserHostAddress);
            var allowExecute = false;

            if (HttpRuntime.Cache[key] == null)
            {
                HttpRuntime.Cache.Add(key,
                    true, // is this the smallest data we can have?
                    null, // no dependencies
                    DateTime.Now.AddSeconds(Seconds), // absolute expiration
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null); // no callback

                allowExecute = true;
            }

            if (!allowExecute)
            {
                if (String.IsNullOrEmpty(Message))
                    Message = "You may only perform this action every {n} seconds.";

                c.Result = new ContentResult { Content = Message.Replace("{n}", Seconds.ToString()) };
                // see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
                c.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
        }
    }
}