namespace Serenity.ComponentModel;

/// <summary>
/// Default implementation for IFilenameFormatSanitizer
/// </summary>
public class DefaultFilenameFormatSanitizer : IFilenameFormatSanitizer
{
    /// <summary>
    /// An instance of this class
    /// </summary>
    public static readonly IFilenameFormatSanitizer Instance = new DefaultFilenameFormatSanitizer();

    /// <summary>
    /// Default implementation for sanitizing values of replacement placeholders 
    /// in a file name format string like |X|/|Y|. Trims the value, 
    /// if is empty, returns "_". , Characters like '/', '&amp;', and diacricits 
    /// etc are replaced by calling StringHelper.SanitizeFileName, 
    /// then replacing backslashes with underscore, double dots and tailing dots with underscore
    /// </summary>
    /// <param name="_">Key for placeholder, ignored by this implementation.</param>
    /// <param name="value">Value to be sanitized</param>
    public virtual string SanitizePlaceholder(string _, string? value)
    {
        value = StringHelper.SanitizeFilename((value ?? "").Replace('\\', '/'));
        value = value.Replace("..", "_", StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(value))
            value = "_";
        while (value.EndsWith(".", StringComparison.Ordinal))
            value = value[..^1] + "_";
        return value;
    }

    /// <summary>
    /// Sanitizing an upload filename formatting result
    /// by replacing backslashes with forward slashes,
    /// and replacing double slashes with "/_/".
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public virtual string SanitizeResult(string result)
    {
        if (string.IsNullOrEmpty(result))
            return result;

        result = result.Replace('\\', '/');

        while (result.IndexOf("//", StringComparison.Ordinal) >= 0)
            result = result.Replace("//", "/_/", StringComparison.Ordinal);

        return result;
    }
}