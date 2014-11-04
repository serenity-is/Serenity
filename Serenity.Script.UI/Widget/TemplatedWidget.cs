using jQueryApi;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serenity
{
    public abstract class TemplatedWidget<TOptions> : Widget<TOptions>
        where TOptions: class, new()
    {
        protected string idPrefix;

        protected TemplatedWidget(jQueryObject element, TOptions opt = null)
            : base(element, opt)
        {
            idPrefix = this.uniqueName + "_";

            string widgetMarkup = GetTemplate().Replace(new Regex("~_", "g"), idPrefix);
            widgetMarkup = JsRender.Render(widgetMarkup);

            this.element.Html(widgetMarkup);
        }

        public jQueryObject ById(string id)
        {
            return J("#" + idPrefix + id);
        }

        public TWidget ById<TWidget>(string id)
            where TWidget: Widget
        {
            return ById(id).GetWidget<TWidget>();
        }

        protected virtual string GetTemplateName()
        {
            return this.GetType().Name;
        }

        protected virtual string GetTemplate()
        {
            string templateName = this.GetTemplateName();
            string template;
            
            var script = J("script#Template_" + templateName);
            if (script.Length > 0)
                template = script.GetHtml();
            else
            {
                #pragma warning disable 618
                template = Q.GetTemplate(templateName);
                #pragma warning restore 618

                if (!Script.IsValue(template))
                    throw new Exception(String.Format(
                        "Can't locate template for widget '{0}' with name '{1}'!", this.GetType().Name, templateName));
            }

            return template;
        }
    }

    public abstract class TemplatedWidget : TemplatedWidget<object>
    {
        protected TemplatedWidget(jQueryObject element)
            : base(element, null)
        {

        }
    }
}