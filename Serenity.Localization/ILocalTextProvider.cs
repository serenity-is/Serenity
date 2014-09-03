namespace Serenity.Localization
{
    public interface ILocalTextProvider
    {
        string TryGet(string key);
        void Add(LocalTextEntry entry, bool pendingApproval);
    }
}