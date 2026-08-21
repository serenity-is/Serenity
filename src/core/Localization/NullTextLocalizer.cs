namespace Serenity;

/// <summary>
/// A text localizer that returns <c>null</c> for all keys.
/// </summary>
public class NullTextLocalizer : ITextLocalizer
{
    private NullTextLocalizer()
    {
    }

    /// <summary>
    /// The singleton instance of the <see cref="NullTextLocalizer"/>.
    /// </summary>
    public static readonly NullTextLocalizer Instance = new();

    /// <summary>
    /// Returns <c>null</c> for all keys.
    /// </summary>
    /// <param name="key">The local text key.</param>
    /// <returns><c>null</c>.</returns>
    public string? TryGet(string key)
    {
        return null;
    }
}