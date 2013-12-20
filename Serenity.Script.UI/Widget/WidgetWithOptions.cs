using jQueryApi;
using System;

namespace Serenity
{
    public abstract class Widget<TOptions> : Widget
        where TOptions: class, new()
    {
        protected readonly TOptions options;

        protected Widget(jQueryObject element, TOptions opt = null)
            : base(element)
        {
            this.options = (opt ?? new TOptions());
        }
    }
}