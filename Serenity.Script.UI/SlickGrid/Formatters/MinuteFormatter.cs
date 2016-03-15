using jQueryApi;
using Serenity.ComponentModel;
using System;

namespace Serenity
{
    public class MinuteFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            return Format(ctx.Value);
        }

        public static string Format(object value)  // 730 = 12:10
        {
            int hour = (int)Math.Floor(value.As<double>()/60);
            int minute = value.As<Int32>() - (hour * 60);

            string hourStr, minuteStr;

            if (!Script.IsValue(value) ||
                Double.IsNaN(value.As<double>()))
                return "";

            if (hour < 10)
                hourStr = "0" + hour;
            else
                hourStr = hour.ToString();

            if (minute < 10)
                minuteStr = "0" + minute;
            else
                minuteStr = minute.ToString();
            
            return String.Format("{0}:{1}", hourStr, minuteStr);
        }
    }
}