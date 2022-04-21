namespace Serenity.Web
{
    public interface IDynamicScript
    {
        string GetScript();
        void CheckRights(IPermissionService permissions, ITextLocalizer localizer);
        string GroupKey { get; }
        TimeSpan Expiration { get; }
    }
}