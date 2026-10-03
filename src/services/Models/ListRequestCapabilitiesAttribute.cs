namespace Serenity.Services;

/// <summary>
/// Specifies the ListRequest properties supported by a list endpoint action.
/// </summary>
/// <remarks>
/// By default, this replaces the standard supported properties. Set <see cref="Exclude"/>
/// to remove the listed properties from the default capabilities instead.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ListRequestCapabilitiesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListRequestCapabilitiesAttribute"/> class.
    /// </summary>
    /// <param name="capabilities">The supported ListRequest property names.</param>
    public ListRequestCapabilitiesAttribute(params string[] capabilities)
    {
        Capabilities = capabilities?.ToArray() ??
            throw new ArgumentNullException(nameof(capabilities));

        if (Capabilities.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Capability names cannot be null or whitespace.", nameof(capabilities));
    }

    /// <summary>
    /// Gets the supported ListRequest property names.
    /// </summary>
    public string[] Capabilities { get; }

    /// <summary>
    /// Gets or sets whether the listed capabilities should be excluded from the
    /// default capabilities instead of being used as the complete supported list.
    /// </summary>
    public bool Exclude { get; set; }
}
