using jQueryApi;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, ScriptName("Widget"), IncludeGenericArguments(false)]
    public abstract class Widget<TOptions> : Widget
        where TOptions: class, new()
    {
        protected new TOptions options
        {
            [InlineCode("{this}.options")]
            get { return null; }
        }

        protected Widget(jQueryObject element, TOptions opt = null)
            : base(element, opt)
        {
        }
    }
}