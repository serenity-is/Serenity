using jQueryApi;
using System;

namespace Serenity
{
    public class DateFormatter : ISlickFormatter
    {
        public DateFormatter()
        {
            DisplayFormat = Q.Culture.DateFormat;
        }

        [Option]
        public string DisplayFormat { get; set; }

        public string Format(SlickFormatterContext ctx)
        {
            return Format(ctx.Value, DisplayFormat);
        }

        public static string Format(object value, string format)
        {
            if (!Script.IsValue(value))
                return "";

            JsDate date;

            if (Script.TypeOf(value) == "date")
                date = value.As<JsDate>();
            else if (Script.TypeOf(value) == "string")
            {
                date = Q.Externals.ParseISODateTime(value.As<string>());
                if (date == null)
                    return Q.HtmlEncode(value.As<string>());
            }
            else
                return value.ToString();

            return Q.HtmlEncode(Q.FormatDate(date, format));

        }
    }
}