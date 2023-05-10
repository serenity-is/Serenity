
namespace Serenity.Localization;

/// <summary>
/// An interface implemented by LocalText class to access initial translation value.
/// Some classes like NestedLocalTextRegistration uses this type to avoid re-registering
/// a initialized local text object, when their Initialization method called more than once.
/// </summary>
public interface ILocalText
{
    /// <summary>
    /// Gets the key
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Returns original key before the replacement / initialization
    /// </summary>
    string? OriginalKey { get; }

    /// <summary>
    /// Replaces local text key with the new key and stores the original key as initial text
    /// </summary>
    /// <param name="newKey">New key</param>
    void ReplaceKey(string newKey);
}