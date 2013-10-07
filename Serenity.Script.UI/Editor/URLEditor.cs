using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("URL")]
    public class URLEditor : StringEditor
    {
        public URLEditor(jQueryObject input)
            : base(input)
        {
            input.AddClass("url").Attribute("title",
                "URL 'http://www.site.com/sayfa' formatında girilmelidir.");
        }
    }
}