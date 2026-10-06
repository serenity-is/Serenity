namespace Serenity.Services;

/// <summary>
/// Optional capability for an <see cref="IRequestContext"/> implementation to provide
/// <see cref="RequestHandlerSettings"/>. When the context does not implement this interface,
/// <see cref="RequestHandlerSettings.Default"/> is used.
/// </summary>
public interface IHasHandlerSettings
{
    /// <summary>
    /// Gets the request handler settings.
    /// </summary>
    RequestHandlerSettings HandlerSettings { get; }
}
