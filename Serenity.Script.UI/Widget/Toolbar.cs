using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public class Toolbar : Widget<ToolbarOptions>
    {
        public Toolbar(jQueryObject div, ToolbarOptions options)
            : base(div, options)
        {
            this.element.AddClass("s-Toolbar")
                .Html("<div class=\"tool-buttons\"><div class=\"buttons-outer\"><div class=\"buttons-inner\"></div></div></div>");

            var container = jQuery.Select("div.buttons-inner", this.element);
        
            for (var i = 0; i < this.options.Buttons.Count; i++) 
            {
                var b = this.options.Buttons[i];

                var cssClass = b.CssClass ?? "";

                var button = jQuery.FromHtml(
                        "<div class=\"tool-button\">" + 
                            "<div class=\"button-outer\">" + 
                                "<span class=\"button-inner\"></span>" + 
                            "</div>" + 
                        "</div>")
                    .AppendTo(container);

                var span = button.Find("span")
                        .AddClass(cssClass)
                        .Click(b.OnClick);

                var text = b.Title;
                if (b.HtmlEncode != false)
                    text = Q.HtmlEncode(b.Title);

                span.Html(text);
            }
        }

        public override void Destroy()
        {
            this.element.Find("span.button-inner").Unbind("click");

            base.Destroy();
        }

        public jQueryObject FindButton(string className)
        {
            return jQuery.Select("span.button-inner." + className, this.element)
                .Closest("div.tool-button");
        }
    }

    [Imported, Serializable]
    public class ToolbarOptions
    {
        public List<ToolButton> Buttons;
    }

    [Imported, Serializable]
    public class ToolButton
    {
        public string Title { get; set; }
        public string CssClass { get; set; }
        public jQueryEventHandler OnClick { get; set; }
        public bool? HtmlEncode { get; set; }
    }
}