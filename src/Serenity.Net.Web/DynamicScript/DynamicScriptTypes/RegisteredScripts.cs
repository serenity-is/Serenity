using System.IO;
using System.Text.Json;

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
        using var ms = new MemoryStream();
        var writer = new Utf8JsonWriter(ms);
        writer.WriteStartObject();
        foreach (var pair in dictionary)
        {
            if (pair.Value == null)
                writer.WriteNumber(pair.Key, 0);
            else
                writer.WriteString(pair.Key, pair.Value);
        }
        writer.WriteEndObject();
        writer.Flush();
        return Encoding.UTF8.GetString(ms.ToArray());
    }
}