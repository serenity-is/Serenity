namespace Serenity.Extensions;

/// <summary>
/// An external login provider that a user can confirm a sensitive action with, instead of the password, see
/// <see cref="IExternalElevationProviders"/>.
/// </summary>
public class ExternalElevationProvider
{
    /// <summary>The name of the provider, e.g. "Google".</summary>
    public string? Name { get; set; }

    /// <summary>The name of the provider to show.</summary>
    public string? DisplayName { get; set; }
}
