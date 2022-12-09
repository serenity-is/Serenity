namespace Serenity.ComponentModel;

/// <summary>
/// Abstraction for sanitizing values of replacement placeholders 
/// in a file name format string like |X|/|Y|. Sanitizer is called 
/// for each of the placeholders. Can be implemented by
/// custom subclasses of upload editor attributes.
/// </summary>
public interface ISanitizeFilenamePlaceholder
{
    /// <summary>
    /// Sanitizes the replacement value
    /// </summary>
    /// <param name="key">Key for replacement parameter</param>
    /// <param name="value">Value for replacement parameter</param>
    string Sanitize(string key, string value);
}