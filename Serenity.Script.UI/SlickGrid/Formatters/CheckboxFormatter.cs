using jQueryApi;
using System;

namespace Serenity
{
    public class CheckboxFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            return "<span class=\"check-box no-float readonly " + (Q.IsTrue(ctx.Value) ? " checked" : "") + "\"></span>";
        }
    }
}