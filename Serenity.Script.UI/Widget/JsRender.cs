using jQueryApi;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static class JsRender
    {
        [InlineCode("Q.jsRender(markup, data)")]
        public static string Render(string markup, object data = null)
        {
            if (markup == null || markup.IndexOf("{{") < 0)
                return markup;

            if (Script.IsNullOrUndefined(jQuery.Instance.templates) ||
                Script.IsNullOrUndefined(jQuery.Instance.views))
            {
                throw new Exception("Please make sure that jsrender.js is included in the page!");
            }
            data = data ?? new JsDictionary();
            var template = jQuery.Instance.templates(markup);
            jQuery.Instance.views.converters(new
            {
                text = new Func<string, string>(s => Q.Text(s))
            }, template);

            return template.render(data);
        }
    }
}