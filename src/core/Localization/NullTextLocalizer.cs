namespace Serenity;

/// <summary>
/// Null text localizer which returns null for all keys
/// </summary>
public class NullTextLocalizer : ITextLocalizer
{
    private NullTextLocalizer()
    {
    }

    /// <summary>
    /// Null text localizer instance
    /// </summary>
    public static readonly NullTextLocalizer Instance = new();

    /// <summary>
    /// Returns null for all keys
    /// </summary>
    /// <param name="key">Local text key</param>
    /// <returns>Null</returns>
    public string? TryGet(string key)
    {
        return null;
    }
}