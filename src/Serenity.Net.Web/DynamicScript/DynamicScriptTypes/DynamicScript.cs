namespace Serenity.Web
{
    public abstract class DynamicScript : IDynamicScript
    {
        protected DynamicScript()
        {
        }

        public abstract string GetScript();

        public virtual void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
        {
            if (Permission != null)
                permissions.ValidatePermission(Permission, localizer);
        }

        public string GroupKey { get; set; }
        public TimeSpan Expiration { get; set; }
        public string Permission { get; set; }
    }
}