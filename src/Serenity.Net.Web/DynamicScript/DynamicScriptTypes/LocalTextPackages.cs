namespace Serenity.Web;

/// <summary>
/// Corresponds to LocalTextPackages section of the appsettings.json file
/// </summary>
public class LocalTextPackages : Dictionary<string, string>
{
    /// <summary>
    /// Default section key
    /// </summary>
    public const string SectionKey = "LocalTextPackages";

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public LocalTextPackages()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }
}