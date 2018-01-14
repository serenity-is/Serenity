using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class NumberFormatter : ISlickFormatter
    {
        [Option, IntrinsicProperty]
        public string DisplayFormat { get; set; }

        public string Format(SlickFormatterContext ctx)
        {
            return Format(ctx.Value, DisplayFormat);
        }

        public static string Format(object value, string format)
        {
            format = format ?? "0.##";

            if (!Script.IsValue(value))
                return "";

            if (Script.TypeOf(value) == "number")
            {
                if (Double.IsNaN(value.As<double>()))
                    return "";

                return Q.HtmlEncode(Q.FormatNumber(value.As<double>(), format));
            }

            var dbl = Q.ParseDecimal(value.ToString());
            if (dbl == null)
                return "";

            return Q.HtmlEncode(value.ToString());
        }
    }
}