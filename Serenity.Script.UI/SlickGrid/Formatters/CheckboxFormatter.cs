using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class CheckboxFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            return "<span class=\"check-box no-float readonly " + (Q.IsTrue(ctx.Value) ? " checked" : "") + "\"></span>";
        }
    }
}