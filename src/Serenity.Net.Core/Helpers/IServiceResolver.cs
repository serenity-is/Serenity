namespace Serenity;

/// <summary>
/// A generic version of IServiceProvider which resolves a service on demand.
/// </summary>
/// <typeparam name="TService"></typeparam>
public interface IServiceResolver<TService>
    where TService : notnull
{
    /// <summary>
    /// Resolves TService using the service provider. If the service was registered as transient, this method acts like a factory.
    /// </summary>
    /// <returns>TService instance</returns>
    TService Resolve();
}
