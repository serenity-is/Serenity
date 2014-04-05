using jQueryApi;

namespace Serenity
{
    public class PrefixedContext : ScriptContext
    {
        private string idPrefix;

        public PrefixedContext(string idPrefix)
        {
            this.idPrefix = idPrefix;
        }

        public jQueryObject ById(string id)
        {
            return J("#" + idPrefix + id);
        }

        public TWidget ById<TWidget>(string id)
            where TWidget : Widget
        {
            return ById(id).GetWidget<TWidget>();
        }

        public string IdPrefix { get { return idPrefix; } }
    }
}
