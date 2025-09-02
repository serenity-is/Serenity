namespace Serenity.ComponentModel;

/// <summary>
/// Abstraction for sanitizing values of replacement placeholders 
/// in a file name format string like |X|/|Y|. Sanitizer is called 
/// for each of the placeholders and for the result. 
/// Can be implemented by custom subclasses of upload editor 
/// attributes or registered in DI.
/// </summary>
public interface IFilenameFormatSanitizer
{
    /// <summary>
    /// Sanitizes the placeholder value
    /// </summary>
    /// <param name="key">Key for placeholder, e.g. field name</param>
    /// <param name="value">Value for placeholder, e.g. field value</param>
    string SanitizePlaceholder(string key, string? value);

    /// <summary>
    /// Sanitizes the formatting result, usually by replacing double
    /// slashes resulting from an empty replacement value with 
    /// /_/ to keep expected directory structure.
    /// </summary>
    /// <param name="result"></param>
    string SanitizeResult(string result);
}