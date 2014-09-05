namespace Serenity.Configuration
{
    using System.Collections;
    using System.Threading;
    using System.Web;

    /// <summary>
    ///   Provides a context item store like HttpContext.Current does.</summary>
    /// <remarks>
    ///   Actually it uses HttpContext.Current.Items for web request threads, but for desktop applications, 
    ///   and threads spawned from web requests, it uses thread local storage. Currently, this context
    ///   is used to store current transaction for connections (<see cref="Serenity.Data.SqlTransactions" />),
    ///   and impersonation stack in security helper (<see cref="Serenity.SecurityHelper" />).
    ///   This might be a flawed design in some cases (so does HttpContext.Current), if a worker thread
    ///   spawns a background worker and passes an IDbConnection to it (as current transaction for 
    ///   that connection won't be passed to the spawned thread context!), or expects impersonation 
    ///   stack to work across thread boundaries.</remarks>
    public class ContextItems
    {
        /// <summary>
        ///   Internal hashtable for non-web applications or spawned threads in web applications.</summary>
        private static ThreadLocal<Hashtable> _internalItems = new ThreadLocal<Hashtable>(
            () => new Hashtable());

        /// <summary>
        ///   Gets a value by its key from current HTTP context items, or if it is not a web application, from the
        ///   thread local hash table.</summary>
        /// <param name="key">
        ///   Key object (required).</param>
        /// <returns>
        ///   Key value.</returns>
        public static object Get(object key)
        {
            var context = HttpContext.Current;
            if (context == null)
                return _internalItems.Value[key];
            return context.Items[key];
        }

        /// <summary>
        ///   Gets a value by its key from current HTTP context items, or if it is not a web application, from the
        ///   thread local hash table.</summary>
        /// <param name="key">
        ///   Key object (required).</param>
        /// <param name="defaultValue">
        ///   Default value to return if the value is null.</param>
        /// <returns>
        ///   Key value.</returns>
        public static T Get<T>(object key, T defaultValue)
        {
            var context = HttpContext.Current;

            object value;
            if (context == null)
                value = _internalItems.Value[key];
            else
                value = context.Items[key];

            if (value == null)
                return defaultValue;
            else
                return (T)(value);
        }

        /// <summary>
        ///   Sets a value by its key in current HTTP context items, or if it is not a web application, in the
        ///   thread local hash table.</summary>
        /// <param name="key">
        ///   Key object (required).</param>
        /// <param name="value">
        ///   Key value.</param>
        public static void Set(object key, object value)
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                if (value == null)
                    _internalItems.Value.Remove(key);
                else
                    _internalItems.Value[key] = value;
            }
            else
            {
                if (value == null)
                    context.Items.Remove(key);
                else
                    context.Items[key] = value;
            }
        }
    }
}
