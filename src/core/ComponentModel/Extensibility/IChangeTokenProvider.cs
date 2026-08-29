using Microsoft.Extensions.Primitives;

namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for an object that can provide a change token that fires
/// when its content changes. The object itself does not need to detect
/// changes; external code may trigger the token and consumers subscribe
/// to it to invalidate their caches.
/// </summary>
public interface IChangeTokenProvider
{
    /// <summary>
    /// Gets a change token that fires when the object's content changes.
    /// </summary>
    IChangeToken GetChangeToken();
}