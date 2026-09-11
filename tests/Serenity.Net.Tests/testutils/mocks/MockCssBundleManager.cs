namespace Serenity.TestUtils;

public class MockCssBundleManager : ICssBundleManager
{
    public bool IsEnabled { get; set; }
    public Func<string, string>? GetCssBundleCallback { get; set; }
    public Dictionary<string, List<string>> Includes { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string GetCssBundle(string cssUrl) => GetCssBundleCallback?.Invoke(cssUrl) ?? cssUrl;

    public IEnumerable<string> GetBundleIncludes(string bundleKey)
    {
        return Includes.TryGetValue(bundleKey, out var includes) ? includes : [];
    }

    public int CssChangedCalls { get; private set; }

    public void CssChanged()
    {
        CssChangedCalls++;
    }

    public void Reset()
    {
    }
}
