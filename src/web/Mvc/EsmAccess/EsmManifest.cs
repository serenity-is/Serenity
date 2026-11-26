using System.Text.Json;

namespace Serenity.Web;

/// <summary>
/// The contents of the <c>build.esm-manifest</c> file that tsbuild writes to an <c>esm</c> folder.
/// </summary>
public class EsmManifest
{
    /// <summary>
    /// The name of the manifest file in an <c>esm</c> folder.
    /// </summary>
    public const string FileName = "build.esm-manifest";

    /// <summary>
    /// The manifest format version.
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>
    /// The entry point output paths, relative to the <c>esm</c> folder.
    /// </summary>
    [JsonPropertyName("entries")]
    public string[] Entries { get; set; } = [];

    /// <summary>
    /// The output paths relative to the <c>esm</c> folder, mapped to the indexes of the
    /// entries (in <see cref="Entries"/>) that reach them.
    /// </summary>
    [JsonPropertyName("files")]
    public Dictionary<string, int[]> Files { get; set; } = [];

    /// <summary>
    /// Parses a manifest.
    /// </summary>
    /// <param name="json">The manifest JSON</param>
    public static EsmManifest Parse(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        var manifest = JsonSerializer.Deserialize<EsmManifest>(json) ?? new EsmManifest();
        manifest.Entries ??= [];
        manifest.Files = new Dictionary<string, int[]>(manifest.Files ?? [], StringComparer.OrdinalIgnoreCase);
        return manifest;
    }
}
