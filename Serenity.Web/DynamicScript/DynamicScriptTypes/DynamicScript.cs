using System;

namespace Serenity.Web
{
    public abstract class DynamicScript : IDynamicScript
    {
        private EventHandler scriptChanged;

        protected DynamicScript()
        {
        }

        public void Changed()
        {
            if (scriptChanged != null)
                scriptChanged(this, new EventArgs());
        }

        public abstract string GetScript();

        public virtual void CheckRights()
        {
            if (Permission == null || Permission == "*")
                return;

            Authorization.ValidateLoggedIn();

            if (Permission == "?" || Permission == "")
                return;
            
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
    }
}