namespace Serenity.TestUtils;

public class MockContentHashCache : IContentHashCache
{
    public Func<PathString, string, string>? ResolveWithHashCallback { get; set; }
    public Func<PathString, string, string>? ResolvePathCallback { get; set; }

    public string ResolvePath(PathString pathBase, string contentPath)
    {
        return ResolvePathCallback?.Invoke(pathBase, contentPath) ?? contentPath;
    }

    public string ResolveWithHash(PathString pathBase, string contentUrl)
    {
        return ResolveWithHashCallback?.Invoke(pathBase, contentUrl) ?? contentUrl;
    }

    public int ScriptsChangedCalls { get; private set; }

    public void ScriptsChanged()
    {
        ScriptsChangedCalls++;
    }
}
