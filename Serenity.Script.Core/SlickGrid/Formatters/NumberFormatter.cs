using jQueryApi;
using Serenity.ComponentModel;
using System;

namespace Serenity
{
    public class NumberFormatter : ISlickFormatter
    {
        [Option]
        public string DisplayFormat { get; set; }

        public string Format(SlickFormatterContext ctx)
        {
            return Format(ctx.Value, DisplayFormat);
        }

        public static string Format(object value, string format)
        {
            format = format ?? "";

            if (!Script.IsValue(value) ||
                Double.IsNaN(value.As<double>()))
                return "";

            if (Script.TypeOf(value) == "number")
                return Q.HtmlEncode(Q.FormatNumber(value.As<double>(), format));

            var dbl = Q.ParseDecimal(value.ToString());
            if (dbl == null)
                return "";

            return Q.HtmlEncode(value.ToString());
        }
    }
}