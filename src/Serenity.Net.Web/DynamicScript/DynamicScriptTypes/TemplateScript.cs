namespace Serenity.Web;

/// <summary>
/// Dynamic script type for HTML templates
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="key">Template key</param>
/// <param name="getTemplate">Get template callback</param>
/// <exception cref="ArgumentNullException"></exception>
public class TemplateScript(string key, Func<string> getTemplate) : DynamicScript, INamedDynamicScript
{
    private readonly string key = key ?? throw new ArgumentNullException(nameof(key));
    private readonly Func<string> getTemplate = getTemplate ?? throw new ArgumentNullException(nameof(getTemplate));

    /// <inheritdoc/>
    public string ScriptName => "Template." + key;

    /// <inheritdoc/>
    public override string GetScript()
    {
        string templateText = getTemplate();

        return string.Format(CultureInfo.InvariantCulture, DataScript.SetScriptDataFormat, 
            ("Template." + key).ToSingleQuoted(),
            templateText.ToSingleQuoted()); 
    }
}