using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serenity
{
    public abstract class TemplatedWidget<TOptions> : Widget<TOptions>
        where TOptions: class, new()
    {
        protected string idPrefix;
        private static JsDictionary<string, string> templateNames = new JsDictionary<string, string>();

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
            Func<string, string> noGeneric = s =>
            {
                var dollar = s.IndexOf("$");
                if (dollar >= 0)
                    return s.Substr(0, dollar);

                return s;
            };

            var fullName = this.GetType().FullName;

            var cachedName = templateNames[fullName];
            if (cachedName != null)
                return cachedName;

            var type = this.GetType();

            while (type != null && type != typeof(Widget))
            {
                var name = noGeneric(type.FullName.Replace(".", "_"));

                foreach (var k in Q.Config.RootNamespaces)
                    if (name.StartsWith(k + "_"))
                    {
                        name = name.Substr(k.Length + 1);
                        break;
                    }

                if (Q.CanLoadScriptData("Template." + name) ||
                    J("script#Template_" + name).Length > 0)
                {
                    templateNames[fullName] = name;
                    return name;
                }

                name = noGeneric(type.Name);

                if (Q.CanLoadScriptData("Template." + name) ||
                    J("script#Template_" + name).Length > 0)
                {
                    templateNames[fullName] = name;
                    return name;
                }

                type = type.BaseType;
            }

            templateNames[fullName] = cachedName = noGeneric(this.GetType().Name);

            return cachedName;
        }

        protected virtual string GetTemplate()
        {
            string templateName = this.GetTemplateName();
            string template;
            
            var script = J("script#Template_" + templateName);
            if (script.Length > 0)
                return script.GetHtml();

            #pragma warning disable 618
            template = Q.GetTemplate(templateName);
            #pragma warning restore 618

            if (!Script.IsValue(template))
                throw new Exception(String.Format(
                    "Can't locate template for widget '{0}' with name '{1}'!", this.GetType().Name, templateName));

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