namespace Serenity.Web;

/// <summary>
/// Dynamic script type for HTML templates
/// </summary>
public class TemplateScript : DynamicScript, INamedDynamicScript
{
    private readonly string key;
    private readonly Func<string> getTemplate;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="key">Template key</param>
    /// <param name="getTemplate">Get template callback</param>
    /// <exception cref="ArgumentNullException"></exception>
    public TemplateScript(string key, Func<string> getTemplate)
    {
        this.getTemplate = getTemplate ?? throw new ArgumentNullException(nameof(getTemplate));
        this.key = key ?? throw new ArgumentNullException(nameof(key));
    }

    /// <inheritdoc/>
    public string ScriptName => "Template." + key;

    /// <inheritdoc/>
    public override string GetScript()
    {
        string templateText = getTemplate();

        return string.Format(CultureInfo.InvariantCulture, "Q.ScriptData.set({0}, {1})", 
            ("Template." + key).ToSingleQuoted(),
            templateText.ToSingleQuoted()); 
    }
}