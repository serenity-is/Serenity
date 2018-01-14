using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class DateTimeFormatter : DateFormatter
    {
        public DateTimeFormatter()
        {
            DisplayFormat = Q.Culture.DateTimeFormat;
        }
    }
}