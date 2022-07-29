namespace Serenity.Web
{
    public interface IDynamicScript
    {
        string GetScript(DynamicScriptResponseType responseType);
        void CheckRights(IPermissionService permissions, ITextLocalizer localizer);
        string GroupKey { get; }
        TimeSpan Expiration { get; }
    }
}