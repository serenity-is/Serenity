using Microsoft.Extensions.Options;

namespace Serenity.Data;

/// <summary>
/// Connection string options
/// </summary>
[DefaultSectionKey(SectionKey)]
public class ConnectionStringOptions : Dictionary<string, ConnectionStringEntry>, 
    IOptions<ConnectionStringOptions>
{
    /// <summary>
    /// Default sectionkey for ConnectionStringOptions
    /// </summary>
    public const string SectionKey = "Data";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public ConnectionStringOptions()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    /// Returns this
    /// </summary>
    public ConnectionStringOptions Value => this;
}