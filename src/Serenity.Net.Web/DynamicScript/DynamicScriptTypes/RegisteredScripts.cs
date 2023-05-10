using System.IO;

namespace Serenity.Web;

/// <summary>
/// A dynamic script type for registered scripts
/// </summary>
public class RegisteredScripts : DynamicScript, INamedDynamicScript, IGetScriptData
{
    private readonly IDynamicScriptManager scriptManager;

    /// <inheritdoc/>
    public string ScriptName => "RegisteredScripts";

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="scriptManager"></param>
    public RegisteredScripts(IDynamicScriptManager scriptManager)
    {
        Expiration = TimeSpan.FromDays(-1);
        this.scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
    }

    /// <inheritdoc/>
    public object GetScriptData()
    {
        return scriptManager.GetRegisteredScripts();
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        return "Q.ScriptData.setRegisteredScripts(" +
            ToJsonFast(scriptManager.GetRegisteredScripts()) + ");";
    }

    private static string ToJsonFast(IDictionary<string, string> dictionary)
    {
        var sw = new StringWriter();
        var writer = new JsonTextWriter(sw);
        writer.WriteStartObject();
        foreach (var pair in dictionary)
        {
            writer.WritePropertyName(pair.Key);
            if (pair.Value == null)
                writer.WriteValue(0);
            else
                writer.WriteValue(pair.Value);
        }
        writer.WriteEndObject();
        return sw.ToString();
    }
}