using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class PrefixedContext : ScriptContext
    {
        protected readonly string idPrefix;

        public PrefixedContext(string idPrefix)
        {
            this.idPrefix = idPrefix;
        }

        [ScriptName("byId")]
        public jQueryObject ById(string id)
        {
            return J("#" + idPrefix + id);
        }

        [InlineCode("this.byId({id}).getWidget({TWidget})")]
        public TWidget ById<TWidget>(string id)
            where TWidget : Widget
        {
            return ById(id).GetWidget<TWidget>();
        }

        [ScriptName("w")]
        private object w(string id, Type t)
        {
            return J("#" + idPrefix + id).GetWidget(t);
        }

        public string IdPrefix { [InlineCode("{this}.idPrefix")] get { return idPrefix; } }
    }
}
