using System.Collections;

namespace Serenity.Web
{
    public abstract class LookupScript : DynamicScript, INamedDynamicScript, IGetScriptData
    {
        private readonly Dictionary<string, object> lookupParams;

        protected LookupScript()
        {
            lookupParams = new Dictionary<string, object>();
        }

        protected abstract IEnumerable GetItems();

        public string GetData()
        {
            var result = new Dictionary<string, object>
            {
                { "items", GetItems() },
            };
            foreach (var item in LookupParams)
            {
                result.Add(item.Key, item.Value);
            }

            return result.ToJson();
        }

        public override string GetScript()
        {
            IEnumerable items = GetItems();

            return string.Format(CultureInfo.InvariantCulture, "Q.ScriptData.set({0}, new Q.Lookup({1}, \n{2}\n));",
                ("Lookup." + LookupKey).ToSingleQuoted(), LookupParams.ToJson(), items.ToJson());
        }

        public Dictionary<string, object> LookupParams => lookupParams;

        public string IdField
        {
            get
            {
                if (lookupParams.TryGetValue("idField", out object value) && value != null)
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
        public string ScriptName => "Lookup." + LookupKey;
    }
}