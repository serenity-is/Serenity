namespace Serenity.TestUtils;

public class MockDynamicScriptManager : IDynamicScriptManager
{
    private readonly Dictionary<string, string?> scriptTexts = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> registered = new(StringComparer.OrdinalIgnoreCase);

    public event Action<string>? ScriptChanged;

    public List<string> ChangedNames { get; } = [];
    public List<string> CheckedRights { get; } = [];
    public Dictionary<string, IDynamicScript> Registered { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Func<string, bool, IScriptContent?>? ReadScriptContentCallback { get; set; }
    public Action<string>? CheckScriptRightsCallback { get; set; }
    public Dictionary<string, string> RegisteredScriptsResult { get; set; } = [];

    public void Changed(string name)
    {
        ChangedNames.Add(name);
        ScriptChanged?.Invoke(name);
    }

    public void CheckScriptRights(string name)
    {
        CheckedRights.Add(name);
        CheckScriptRightsCallback?.Invoke(name);
    }

    public Dictionary<string, string> GetRegisteredScripts()
    {
        return RegisteredScriptsResult;
    }

    public IEnumerable<string> GetRegisteredScriptNames() => registered;

    public string GetScriptInclude(string name, string extension = ".js")
    {
        return name + extension;
    }

    public string? GetScriptText(string name, bool json = false)
    {
        return scriptTexts.TryGetValue(name, out var text) ? text : null;
    }

    public IScriptContent? ReadScriptContent(string name, bool json = false)
    {
        return ReadScriptContentCallback?.Invoke(name, json);
    }

    public void IfNotRegistered(string name, Func<IDynamicScript> callback)
    {
        if (!registered.Contains(name))
            Register(name, callback());
    }

    public bool IsRegistered(string name) => registered.Contains(name);

    public void Register(INamedDynamicScript script)
    {
        Register(script.ScriptName, script);
    }

    public void Register(string name, IDynamicScript script)
    {
        registered.Add(name);
        Registered[name] = script;
        if (scriptTexts.ContainsKey(name))
            Changed(name);
    }

    public void Reset()
    {
        registered.Clear();
        scriptTexts.Clear();
        Registered.Clear();
    }

    public void SetScriptText(string name, string? text)
    {
        scriptTexts[name] = text;
        registered.Add(name);
    }
}
