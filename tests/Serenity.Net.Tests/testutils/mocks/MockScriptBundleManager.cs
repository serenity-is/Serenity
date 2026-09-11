namespace Serenity.TestUtils;

public class MockScriptBundleManager : IScriptBundleManager
{
    public bool IsEnabled { get; set; }
    public Func<string, string>? GetScriptBundleCallback { get; set; }
    public Dictionary<string, List<string>> Includes { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string GetScriptBundle(string scriptUrl) => GetScriptBundleCallback?.Invoke(scriptUrl) ?? scriptUrl;

    public IEnumerable<string> GetBundleIncludes(string bundleKey)
    {
        return Includes.TryGetValue(bundleKey, out var includes) ? includes : [];
    }

    public void Reset()
    {
    }

    public int ScriptsChangedCalls { get; private set; }

    public void ScriptsChanged()
    {
        ScriptsChangedCalls++;
    }
}
