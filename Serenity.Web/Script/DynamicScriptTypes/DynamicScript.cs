using System;

namespace Serenity.Web
{
    public class DynamicScript : INamedDynamicScript, ITwoLevelCached
    {
        private EventHandler scriptChanged;

        public DynamicScript()
        {
        }

        public void Changed()
        {
            if (scriptChanged != null)
                scriptChanged(this, new EventArgs());
        }

        public virtual string GetScript()
        {
            Check.NotNull("GetData", "getData");

            object data = GetData();

            return String.Format("Q$ScriptData.set({0}, {1});",
                (ScriptName).ToSingleQuoted(), data.ToJson());
        }

        public void CheckRights()
        {
            if (Permission == null)
                return;

            if (Permission == "?" || Permission == "")
            {
                Authorization.ValidateLoggedIn();
                return;
            }

            if (Permission == "*")
            {
                Authorization.ValidatePermission(Permission);
                return;
            }

            Authorization.ValidateLoggedIn();
            Authorization.ValidatePermission(Permission);
        }

        public event System.EventHandler ScriptChanged
        {
            add { scriptChanged += value; }
            remove { scriptChanged -= value; }
        }

        public string GroupKey { get; set; }
        public TimeSpan Expiration { get; set; }
        public string Permission { get; set; }
        public string ScriptName { get; set; }
        public Func<object> GetData { get; set; }
    }
}