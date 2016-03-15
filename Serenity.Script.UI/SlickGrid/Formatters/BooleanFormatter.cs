using jQueryApi;
using System;

namespace Serenity
{
    public class BooleanFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            if (!Script.IsValue(ctx.Value))
                return "";

            if (Q.IsTrue(ctx.Value))
                return Q.HtmlEncode(Q.TryGetText(TrueText) ?? TrueText ?? Q.TryGetText("Dialogs.YesButton") ?? "Yes");

            return Q.HtmlEncode(Q.TryGetText(FalseText) ?? FalseText ?? Q.TryGetText("Dialogs.NoButton") ?? "No");
        }

        [Option]
        public string FalseText { get; set; }

        [Option]
        public string TrueText { get; set; }
    }
}