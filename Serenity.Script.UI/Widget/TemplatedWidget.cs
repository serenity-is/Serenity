using jQueryApi;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [IncludeGenericArguments(false), ScriptName("TemplatedWidget")]
    [Imported]
    public abstract class TemplatedWidget<TOptions> : Widget<TOptions>
        where TOptions : class, new()
    {
        protected readonly string idPrefix;
        private static JsDictionary<string, string> templateNames = new JsDictionary<string, string>();

        protected TemplatedWidget(jQueryObject element, TOptions opt = null)
            : base(element, opt)
        {
        }

        public string IdPrefix { [InlineCode("{this}.idPrefix")] get { return idPrefix; } }

        [ScriptName("byId")]
        public jQueryObject ById(string id)
        {
            return J("#" + idPrefix + id);
        }

        [ScriptName("byID")]
        [IncludeGenericArguments(false)]
        [InlineCode("{this}.byID({id}, {TWidget})")]
        public TWidget ById<TWidget>(string id)
            where TWidget: Widget
        {
            return null;
        }

        protected virtual string GetTemplateName()
        {
            return null;
        }

        protected virtual string GetTemplate()
        {
            return null;
        }
    }

    [Imported, ScriptName("TemplatedWidget")]
    public abstract class TemplatedWidget : TemplatedWidget<object>
    {
        protected TemplatedWidget(jQueryObject element)
            : base(element, null)
        {

        }
    }
}