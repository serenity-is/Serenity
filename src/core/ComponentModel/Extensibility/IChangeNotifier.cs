namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for an object that can be externally notified that its content
/// has changed. The object itself does not need to detect changes; external
/// code calls <see cref="NotifyChanged"/> and consumers subscribe to the
/// change token to invalidate their caches.
/// </summary>
public interface IChangeNotifier
{
    /// <summary>
    /// Notifies subscribers that the object's content has changed.
    /// </summary>
    void NotifyChanged();
}