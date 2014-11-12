using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Web
{
    public class LookupScript : INamedDynamicScript, ITwoLevelCached
    {
        private EventHandler scriptChanged;
        private Dictionary<string, object> lookupParams;

        public LookupScript()
        {
            lookupParams = new Dictionary<string, object>();
            Expiration = TimeSpan.Zero;
        }

        public void Changed()
        {
            if (scriptChanged != null)
                scriptChanged(this, new EventArgs());
        }

        public string GetScript()
        {
            IEnumerable items = GetItems();

            return String.Format("Q$ScriptData.set({0}, new Q$Lookup({1}, \n{2}\n));",
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

        public void CheckRights()
        {
            if (Authorize && !Authorization.IsLoggedIn)
                throw new AccessViolationException(String.Format("{0} script'ine yalnızca giriş yapmış kullanıcılar tarafından erişilebilir!"));

            if (Permission != null)
                Authorization.ValidatePermission(Permission);
        }

        public event System.EventHandler ScriptChanged
        {
            add { scriptChanged += value; }
            remove { scriptChanged -= value; }
        }

        public string GroupKey { get; set; }
        public TimeSpan Expiration { get; set; }
        public bool Authorize { get; set; }
        public string Permission { get; set; }
        public bool NonCached { get; set; }
        public string LookupKey { get; set; }
        public string ScriptName { get { return "Lookup." + LookupKey; } }
        public Func<IEnumerable> GetItems { get; set; }
    }
}