
namespace Serenity
{
    using jQueryApi;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Html;
    using System.Runtime.CompilerServices;

    [Editor, Element("<div />")]
    public class Recaptcha : Widget<RecaptchaOptions>, IStringValue
    {
        public Recaptcha(jQueryObject div, RecaptchaOptions opt)
            : base(div, opt)
        {
            this.element.AddClass("g-recaptcha")
                .Attribute("data-sitekey", options.SiteKey);

            if (Window.Instance.As<dynamic>().grecaptcha == null &&
                J("script#RecaptchaInclude").Length == 0)
            {
                var src = "https://www.google.com/recaptcha/api.js";
                src += "?hl=" + (options.Language ?? J("html").GetAttribute("lang") ?? "");

                J("<script/>").Attribute("id", "RecaptchaInclude").Attribute("src", src)
                    .AppendTo(Document.Body);
            }

            var input = J("<input />").InsertBefore(this.element).Attribute("id", this.UniqueName + "_validate")
                .Value("x").CSS(new JsDictionary<string, object>
                {
                    { "visibility", "hidden" },
                    { "width", "0px" },
                    { "height", "0px" },
                    { "padding", "0px" }
                });

            var self = this;
            input.AddValidationRule(this.uniqueName, e =>
            {
                if (string.IsNullOrEmpty(this.Value))
                    return Q.Text("Validation.Required");

                return null;
            });
        }

        public string Value
        {
            get
            {
                return this.element.Find(".g-recaptcha-response").GetValue();
            }
            set
            {
                // ignore
            }
        }
    }

    [Imported, Serializable]
    public class RecaptchaOptions
    {
        public string SiteKey { get; set; }
        public string Language { get; set; }
    }
}