namespace Serenity.Web
{
    public interface IScriptBundleManager
    {
        bool IsEnabled { get; }

        IEnumerable<string> GetBundleIncludes(string bundleKey);
        string GetScriptBundle(string scriptUrl);
        void Reset();
        void ScriptsChanged();
    }
}