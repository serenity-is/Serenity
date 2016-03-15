using jQueryApi;
using System;

namespace Serenity
{
    public class DateTimeFormatter : DateFormatter
    {
        public DateTimeFormatter()
        {
            DisplayFormat = Q.Culture.DateTimeFormat;
        }
    }
}