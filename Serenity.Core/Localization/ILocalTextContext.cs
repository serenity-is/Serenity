namespace Serenity.Localization
{
    public interface ILocalTextContext
    {
        int LanguageID { get; set; }
        bool IsApprovalMode { get; set; }
    }
}