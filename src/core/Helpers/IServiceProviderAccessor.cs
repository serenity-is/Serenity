namespace Serenity;

/// <summary>
/// Provides access to the service provider that should be used to resolve or activate services
/// on demand, such as request handlers and behaviors.
/// </summary>
/// <remarks>
/// In web applications the returned provider is the current request's scoped provider when
/// available, falling back to the root provider otherwise. Resolving through this abstraction
/// instead of capturing an <see cref="IServiceProvider"/> in a singleton avoids the captive
/// dependency problem for services that may be registered as scoped.
/// </remarks>
public interface IServiceProviderAccessor
{
    /// <summary>
    /// Gets the service provider to use.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}
