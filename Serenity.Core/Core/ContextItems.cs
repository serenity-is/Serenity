using System.Collections;
using System.Threading;
using System.Web;

namespace Serenity
{
    /// <summary>
    ///   Helper class to access a hashtable for current request, or a global hashtable if this is not
    ///   a web application.</summary>
    public class ContextItems
    {
        /// <summary>
        ///   Internal hashtable for non-web applications or separate threads in web applications.</summary>
        private static ThreadLocal<Hashtable> _internalItems = new ThreadLocal<Hashtable>(
            () => new Hashtable());

        /// <summary>
        ///   Gets a value by its key from current HTTP context items, or if it is not a web application, from the
        ///   global hash table.</summary>
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
        ///   global hash table.</summary>
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
        ///   global hash table.</summary>
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
