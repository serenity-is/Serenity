namespace Serenity.TestUtils;

public class MockCssMinifier : ICssMinifier
{
    public Func<string, CssMinifyOptions, CssMinifyResult>? MinifyCallback { get; set; }

    public CssMinifyResult MinifyCss(string content, CssMinifyOptions options)
    {
        if (MinifyCallback != null)
            return MinifyCallback(content, options);

        return new CssMinifyResult { Code = content };
    }
}
