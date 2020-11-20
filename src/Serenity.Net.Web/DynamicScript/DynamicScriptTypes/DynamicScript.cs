using Serenity.Abstractions;
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

        public virtual void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
        {
            if (Permission != null)
                permissions.ValidatePermission(Permission, localizer);
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