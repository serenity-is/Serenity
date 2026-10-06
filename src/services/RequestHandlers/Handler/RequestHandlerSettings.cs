using Microsoft.Extensions.Options;

namespace Serenity.Services;

/// <summary>
/// Contains settings that are applied to request handlers. Bound from the
/// <c>RequestHandlers</c> section in appsettings.json when available.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class RequestHandlerSettings : IOptions<RequestHandlerSettings>
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "RequestHandlers";

    /// <summary>
    /// Gets the default settings (no limits).
    /// </summary>
    public static readonly RequestHandlerSettings Default = new();

    /// <summary>
    /// Gets or sets the list request settings.
    /// </summary>
    public ListHandlerSettings List { get; set; } = new();

    /// <summary>
    /// Gets this instance.
    /// </summary>
    public RequestHandlerSettings Value => this;
}
