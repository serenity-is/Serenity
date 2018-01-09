using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface ISlickFormatter
    {
        string Format(SlickFormatterContext ctx);
    }
}