using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Serenity.Web
{
    public abstract class LookupScript : DynamicScript, INamedDynamicScript
    {
        private Dictionary<string, object> lookupParams;

        protected LookupScript()
        {
            lookupParams = new Dictionary<string, object>();
        }

        protected abstract IEnumerable GetItems();

        public override string GetScript()
        {
            IEnumerable items = GetItems();

            return string.Format(CultureInfo.InvariantCulture, "Q.ScriptData.set({0}, new Q.Lookup({1}, \n{2}\n));",
                ("Lookup." + LookupKey).ToSingleQuoted(), LookupParams.ToJson(), items.ToJson());
        }

        public Dictionary<string, object> LookupParams
        {
            get { return lookupParams; }
        }

        public string IdField
        {
            get
            {
                object value;
                if (lookupParams.TryGetValue("idField", out value) && value != null)
                    return value.ToString();

                return null;
            }
            set
            {
                lookupParams["idField"] = value;
            }
        }

        public string TextField
        {
            get
            {
                if (lookupParams.TryGetValue("textField", out object value) && value != null)
                    return value.ToString();

                return null;
            }
            set
            {
                lookupParams["textField"] = value;
            }
        }

        public string ParentIdField
        {
            get
            {
                if (lookupParams.TryGetValue("parentIdField", out object value) && value != null)
                    return value.ToString();

                return null;
            }
            set
            {
                lookupParams["parentIdField"] = value;
            }
        }

        public string LookupKey { get; set; }
        public string ScriptName { get { return "Lookup." + LookupKey; } }
    }
}