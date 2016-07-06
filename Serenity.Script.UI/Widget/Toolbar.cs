using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public class Toolbar : Widget<ToolbarOptions>
    {
        public Toolbar(jQueryObject div, ToolbarOptions options)
            : base(div, options)
        {
            this.element.AddClass("s-Toolbar clearfix")
                .Html("<div class=\"tool-buttons\"><div class=\"buttons-outer\">" + 
                    "<div class=\"buttons-inner\"></div></div></div>");

            var container = J("div.buttons-inner", this.element);
        
            var buttons = this.options.Buttons;

            for (var i = 0; i < buttons.Count; i++)
                CreateButton(container, buttons[i]);
        }

        private void CreateButton(jQueryObject container, ToolButton b)
        {
            var cssClass = b.CssClass ?? "";

            if (b.Separator == true)
            {
                J("<div class=\"separator\"></div>")
                    .AppendTo(container);
            }

            var btn = J(
                    "<div class=\"tool-button\">" +
                        "<div class=\"button-outer\">" +
                            "<span class=\"button-inner\"></span>" +
                        "</div>" +
                    "</div>")
                .AppendTo(container);

            if (cssClass.Length > 0)
                btn.AddClass(cssClass);

            if (!b.Hint.IsEmptyOrNull())
                btn.Attribute("title", b.Hint);

            btn.Click(delegate(jQueryEvent e)
            {
                if (btn.HasClass("disabled"))
                    return;

                b.OnClick(e);
            });

            var text = b.Title;
            if (b.HtmlEncode != false)
                text = Q.HtmlEncode(b.Title);

            if (!string.IsNullOrEmpty(b.Icon))
            {
                btn.AddClass("icon-tool-button");

                var klass = b.Icon;
                if (klass.StartsWith("fa-"))
                    klass = "fa " + klass;
                else if (klass.StartsWith("glyphicon-"))
                    klass = "glyphicon " + klass;

                text = "<i class='" + klass + "'></i> " + text;
            }

            if (text == null || text.Length == 0)
                btn.AddClass("no-text");
            else
                btn.Find("span").Html(text);

            if (!string.IsNullOrEmpty(b.Hotkey) &&
                Script.IsValue(Window.Instance.As<dynamic>().Mousetrap))
            {
                Mousetrap.Wrap(options.HotkeyContext ?? Window.Document.DocumentElement).Bind(b.Hotkey, (e, action) =>
                {
                    if (btn.Is(":visible"))
                        btn.TriggerHandler("click");

                    return (b.HotkeyAllowDefault);
                });
            }
        }

        public override void Destroy()
        {
            this.element.Find("div.tool-button").Unbind("click");

            base.Destroy();
        }

        public jQueryObject FindButton(string className)
        {
            if (className != null &&
                className.StartsWith("."))
                className = className.Substr(1);

            return J("div.tool-button." + className, this.element);
        }
    }

    [Imported, Serializable]
    public class ToolbarOptions
    {
        public List<ToolButton> Buttons;
        public Element HotkeyContext;
    }

    [Imported, Serializable]
    public class ToolButton
    {
        public string Title { get; set; }
        public string Hint { get; set; }
        public string CssClass { get; set; }
        public string Icon { get; set; }
        public jQueryEventHandler OnClick { get; set; }
        public bool? HtmlEncode { get; set; }
        public string Hotkey { get; set; }
        public bool HotkeyAllowDefault { get; set; }
        public bool Separator { get; set; }
    }
}