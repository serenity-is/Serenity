namespace Serenity.Web;

/// <summary>
/// Default implementation of IScriptMinifier and ICssMinifer interfaces with Nuglify library
/// </summary>
public class NuglifyMinifier : ICssMinifier, IScriptMinifier
{
    /// <inheritdoc/>
    public CssMinifyResult MinifyCss(string content, CssMinifyOptions options)
    {
        var result = NUglify.Uglify.Css(content, new NUglify.Css.CssSettings()
        {
            LineBreakThreshold = options.LineBreakThreshold == 0 ? 
                int.MaxValue - 1000 : options.LineBreakThreshold
        });

        return new CssMinifyResult
        {
            Code = result.Code,
            HasErrors = result.HasErrors
        };
    }

    /// <inheritdoc/>
    public ScriptMinifyResult MinifyScript(string content, ScriptMinifyOptions options)
    {
        var result = NUglify.Uglify.Js(content, new NUglify.JavaScript.CodeSettings()
        {
            LineBreakThreshold = options.LineBreakThreshold == 0 ?
                int.MaxValue - 1000 : options.LineBreakThreshold
        });

        return new ScriptMinifyResult
        {
            Code = result.Code,
            HasErrors = result.HasErrors
        };
    }
}