namespace Serenity.Extensions;

/// <summary>
/// Settings for ClamAV
/// </summary>
[DefaultSectionKey(SectionKey)]
public class ClamAVSettings
{
    /// <summary>
    /// Default section key for ClamAV
    /// </summary>
    public const string SectionKey = "ClamAV";

    /// <summary>
    /// If ClamAV scanning is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Host to connect to, default is localhost
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Port to connect to, default is 3310
    /// </summary>
    public int Port { get; set; } = 3310;
}