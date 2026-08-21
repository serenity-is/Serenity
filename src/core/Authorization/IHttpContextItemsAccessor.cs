namespace Serenity.Abstractions;

/// <summary>
/// Provides access to the per-request item dictionary, typically backed by
/// <c>HttpContext.Items</c> for web requests.
/// </summary>
/// <remarks>
/// Returns <c>null</c> when accessed outside of a web request context.
/// </remarks>
public interface IHttpContextItemsAccessor
{
    /// <summary>
    /// Gets the dictionary that can be used as request-scoped storage.
    /// </summary>
    IDictionary<object, object?> Items { get; }
}