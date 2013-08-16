using jQueryApi;
using System;

namespace Serenity
{
    public abstract class Widget<TOptions> : Widget
        where TOptions: class, new()
    {
        protected readonly TOptions options;

        protected Widget(jQueryObject element, TOptions opt)
            : base(element)
        {
            var elementOptions = element.GetDataValue(this.GetType().Name).As<TOptions>();
            this.options = jQuery.ExtendObject(new TOptions(), GetDefaults(), elementOptions, opt);
        }

        protected virtual TOptions GetDefaults()
        {
            return new TOptions();
        }
    }
}