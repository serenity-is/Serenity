namespace Serenity.Services;

/// <summary>
/// An interface to provide a text key for localization purposes.
/// </summary>
public interface IHasTextKey
{
    /// <summary>
    /// Gets the text key used for localization purposes.
    /// </summary>
    string? TextKey { get; }
}