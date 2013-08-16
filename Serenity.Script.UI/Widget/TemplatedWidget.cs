using jQueryApi;
using System;
using System.Text.RegularExpressions;

namespace Serenity
{
    public abstract class TemplatedWidget<TOptions> : Widget<TOptions>
        where TOptions: class, new()
    {
        protected string idPrefix;

        protected TemplatedWidget(jQueryObject element, TOptions options)
            : base(element, options)
        {
            idPrefix = this.uniqueName + "_";

            string widgetMarkup = GetTemplate().Replace(new Regex("~_", "g"), idPrefix);

            this.element.Html(widgetMarkup);
        }

        public jQueryObject ById(string id)
        {
            return jQuery.Select("#" + idPrefix + id);
        }

        protected virtual string GetTemplateName()
        {
            return this.GetType().Name;
        }

        protected virtual string GetTemplate()
        {
            string templateName = this.GetTemplateName();
            string template;
            
            var script = jQuery.Select("script#Template_" + templateName);
            if (script.Length > 0)
                template = script.GetHtml();
            else
            {
                template = Q.GetTemplate(templateName);

                if (!Script.IsValue(template))
                    throw new Exception(String.Format(
                        "Can't locate template for widget '{0}' with name '{1}'!", this.GetType().Name, templateName));
            }

            return template;
        }
    }
}