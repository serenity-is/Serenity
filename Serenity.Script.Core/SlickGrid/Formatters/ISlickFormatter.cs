using jQueryApi;
using System;

namespace Serenity
{
    public interface ISlickFormatter
    {
        string Format(SlickFormatterContext ctx);
    }
}