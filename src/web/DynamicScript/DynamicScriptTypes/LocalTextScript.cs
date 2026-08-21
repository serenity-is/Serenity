using System.Text.Json;

namespace Serenity.Web;

/// <summary>
/// Local text dynamic script.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LocalTextScript"/> class.
/// </remarks>
/// <param name="registry">The text registry.</param>
/// <param name="package">The package key.</param>
/// <param name="includes">The includes regex.</param>
/// <param name="languageId">The language ID.</param>
/// <param name="isPending"><c>true</c> to include pending texts.</param>
/// <exception cref="ArgumentNullException"><paramref name="registry"/> or <paramref name="package"/> is <c>null</c>.</exception>
public class LocalTextScript(ILocalTextRegistry registry, string package, string includes, string languageId, bool isPending) : DynamicScript, INamedDynamicScript
{
    private readonly string scriptName = GetScriptName(package, languageId, isPending);
    private readonly string languageId = languageId;
    private readonly string includes = includes;
    private readonly bool isPending = isPending;
    private readonly ILocalTextRegistry registry = registry ?? throw new ArgumentNullException(nameof(registry));
    private readonly string package = package ?? throw new ArgumentNullException(nameof(package));

    /// <inheritdoc/>
    public string ScriptName => scriptName;

    /// <summary>
    /// Gets the script registration name for a local text package.
    /// </summary>
    /// <param name="package">The package key.</param>
    /// <param name="languageId">The language ID.</param>
    /// <param name="isPending">The pending flag.</param>
    /// <returns>The script registration name.</returns>
    public static string GetScriptName(string package, string languageId, bool isPending)
    {
        return string.Format(CultureInfo.InvariantCulture, "LocalText.{0}.{1}.{2}", package, languageId, isPending ? "Pending" : "Public");
    }

    /// <summary>
    /// Gets the local text package script content.
    /// </summary>
    /// <param name="registry">The text registry.</param>
    /// <param name="packages">The packages setting.</param>
    /// <param name="package">The package key.</param>
    /// <param name="languageId">The language ID.</param>
    /// <param name="isPending"><c>true</c> to include pending texts.</param>
    /// <returns>The script content.</returns>
    /// <exception cref="ArgumentNullException">The package key or packages setting is <c>null</c>.</exception>
    public static string GetLocalTextPackageScript(ILocalTextRegistry registry, 
        LocalTextPackages packages, string package, string languageId, bool isPending)
    {
        ArgumentNullException.ThrowIfNull(package);

        ArgumentNullException.ThrowIfNull(packages);

        if (!packages.TryGetValue(package, out string includes))
            includes = null;

        return GetLocalTextPackageScript(registry, includes, languageId, isPending, package);
    }

    /// <summary>
    /// Gets a local text package script content.
    /// </summary>
    /// <param name="registry">The text registry.</param>
    /// <param name="includes">The includes regex.</param>
    /// <param name="languageId">The language ID.</param>
    /// <param name="isPending"><c>true</c> to include pending texts.</param>
    /// <param name="packageId">The package ID.</param>
    /// <returns>The script content.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="registry"/> is <c>null</c>.</exception>
    public static string GetLocalTextPackageScript(ILocalTextRegistry registry, 
        string includes, string languageId, bool isPending, string packageId = null)
    {
        var list = LocalTextDataScript.GetPackageData(registry, includes, languageId, isPending, packageId).ToList();
        list.Sort((i1, i2) => string.CompareOrdinal(i1.Key, i2.Key));

        var ms = new System.IO.MemoryStream();
        var jw = new Utf8JsonWriter(ms);
        jw.WriteStartObject();
        List<string> stack = [];
        int stackCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var pair = list[i];
            // we can't handle keys that ends with '.' for now (possible clash between "site." and "site")
            if (pair.Key.Length == 0 || pair.Key[^1] == '.')
                continue;

            var parts = pair.Key.Split('.');

            if (parts.Length == 0)
                continue;

            int same = 0;

            if (stackCount > 0)
            {
                while (same < stackCount && same < parts.Length && stack[same] == parts[same])
                    same++;

                for (int level = same; level < stackCount; level++)
                    jw.WriteEndObject();

                stackCount = same;
            }

            for (int level = same; level < parts.Length - 1; level++)
            {
                string part = parts[level];
                if (stack.Count > level)
                    stack[level] = part;
                else
                    stack.Add(part);
                jw.WritePropertyName(part);
                jw.WriteStartObject();
            }
            stackCount = parts.Length - 1;

            if (same != parts.Length)
            {
                string part = parts[^1];

                bool nextStartsWithThis = false;
                if (i + 1 < list.Count)
                {
                    var next = list[i + 1];
                    if (next.Key.StartsWith(pair.Key, StringComparison.OrdinalIgnoreCase) &&
                        next.Key.Length > pair.Key.Length &&
                        next.Key[pair.Key.Length] == '.')
                        nextStartsWithThis = true;
                }

                if (nextStartsWithThis)
                {
                    stackCount = parts.Length;
                    if (stack.Count > stackCount - 1)
                        stack[stackCount - 1] = part;
                    else
                        stack.Add(part);
                    jw.WritePropertyName(part);
                    jw.WriteStartObject();
                    jw.WritePropertyName("");
                    jw.WriteStringValue(pair.Value);
                }
                else
                {
                    jw.WritePropertyName(part);
                    jw.WriteStringValue(pair.Value);
                }
            }
        }
        for (int i = 0; i < stackCount; i++)
            jw.WriteEndObject();
        jw.WriteEndObject();
        jw.Flush();

        return "((typeof Serenity!=='undefined'&&Serenity.addLocalText)||(function(l){var s=Symbol.for('Serenity.localTextTable');var g=globalThis[s];if(!g)g=globalThis[s]={};" + 
            "function a(t,p){p||='';for (var k of Object.keys(t)){var u=p+k,o=t[k];if(typeof o==='object'){a(o,u+'.')}else{g[u]=o}}}" + 
            "a(l);}))(" +  Encoding.UTF8.GetString(ms.ToArray()) + ");";
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        return GetLocalTextPackageScript(registry, includes, languageId, isPending, package);
    }  
}