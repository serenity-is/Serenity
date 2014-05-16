namespace Serenity
{
    using jQueryApi;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Base class that provides a shortcut to `jQuery.Select` (`$`) function as `J`
    /// </summary>
    public class ScriptContext
    {
        /// <summary>
        /// Calls jQuery function
        /// </summary>
        [InlineCode("$({p})")]
        protected static jQueryObject J(object p)
        {
            return null;
        }

        /// <summary>
        /// Calls jQuery function with given context
        /// </summary>
        [InlineCode("$({p}, {context})")]
        protected static jQueryObject J(object p, object context)
        {
            return null;
        }
    }
}