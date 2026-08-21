
namespace Serenity.Localization;

/// <summary>
/// An interface implemented by the <see cref="LocalText"/> class to access the initial translation value.
/// Some classes like <see cref="NestedLocalTextRegistration"/> use this type to avoid re-registering
/// an already initialized local text object when their initialization method is called more than once.
/// </summary>
public interface ILocalText
{
    /// <summary>
    /// Gets the local text key.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Returns the original key before the replacement or initialization.
    /// </summary>
    string? OriginalKey { get; }

    /// <summary>
    /// Replaces the local text key with the new key and stores the original key as the initial text.
    /// </summary>
    /// <param name="newKey">The new key.</param>
    void ReplaceKey(string newKey);
}