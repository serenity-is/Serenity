using System;
using System.Collections;
using System.Collections.Generic;

namespace Serenity.Web
{
    public class LookupScript : DynamicScript, INamedDynamicScript
    {
        private Dictionary<string, object> lookupParams;
        protected Func<IEnumerable> getItems;

        protected LookupScript()
        {
            lookupParams = new Dictionary<string, object>();
        }

        public LookupScript(Func<IEnumerable> getItems)
            : this()
        {
            Check.NotNull(getItems, "getItems");
            this.getItems = getItems;
        }

        public override string GetScript()
        {
            IEnumerable items = getItems();

            return String.Format("Q.ScriptData.set({0}, new Q.Lookup({1}, \n{2}\n));",
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
                object value;
                if (lookupParams.TryGetValue("textField", out value) && value != null)
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
                object value;
                if (lookupParams.TryGetValue("parentIdField", out value) && value != null)
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