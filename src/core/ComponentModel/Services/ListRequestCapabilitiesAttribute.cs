namespace Serenity.Services;

/// <summary>
/// Specifies the ListRequest capabilities supported by a list endpoint action.
/// </summary>
/// <remarks>
/// <see cref="Capabilities"/> declares the supported set. <see cref="Exclude"/> removes
/// specific capabilities from that set.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ListRequestCapabilitiesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListRequestCapabilitiesAttribute"/> class.
    /// </summary>
    /// <param name="capabilities">The supported ListRequest capabilities.</param>
    public ListRequestCapabilitiesAttribute(ListRequestCapabilities capabilities)
    {
        Capabilities = capabilities;
    }

    /// <summary>
    /// Gets the supported ListRequest capabilities.
    /// </summary>
    public ListRequestCapabilities Capabilities { get; }

    /// <summary>
    /// Gets or sets the capabilities to remove from <see cref="Capabilities"/>.
    /// </summary>
    public ListRequestCapabilities Exclude { get; set; }
}
