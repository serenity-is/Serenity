namespace Serenity
{
    using jQueryApi;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides a shortcut to `jQuery.Select` (`$`) function as `J`
    /// </summary>
    public class ScriptContext
    {
        [InlineCode("$({p})")]
        protected static jQueryObject J(object p)
        {
            return null;
        }

        [InlineCode("$({p}, {context})")]
        protected static jQueryObject J(object p, object context)
        {
            return null;
        }
    }
}