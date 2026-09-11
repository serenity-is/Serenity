namespace Serenity.TestUtils;

public class MockScriptMinifier : IScriptMinifier
{
    public Func<string, ScriptMinifyOptions, ScriptMinifyResult>? MinifyCallback { get; set; }

    public ScriptMinifyResult MinifyScript(string content, ScriptMinifyOptions options)
    {
        if (MinifyCallback != null)
            return MinifyCallback(content, options);

        return new ScriptMinifyResult { Code = content };
    }
}
