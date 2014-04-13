using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(string value)
        {
            this.Value = value;
        }

        public string Value  { get; private set; }
    }
}