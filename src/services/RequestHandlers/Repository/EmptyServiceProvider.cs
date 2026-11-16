namespace Serenity.Services;

/// <summary>
/// A service provider that has no services, for a request context that is not given the ones of the request.
/// </summary>
internal sealed class EmptyServiceProvider : IServiceProvider
{
    public static readonly EmptyServiceProvider Instance = new();

    public object? GetService(Type serviceType) => null;
}
