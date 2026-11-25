namespace Serenity.Web;

/// <summary>
/// A part of a <see cref="ConcatenatedScript"/>, which may only be included
/// when the current user has the permission for it.
/// </summary>
/// <param name="getScript">Returns the content of the part</param>
/// <param name="hasPermission">Returns true if the current user can access this part.
/// When null, the part is always included.</param>
/// <param name="getCacheSuffix">Returns the cache suffix of the part, e.g. when its
/// content changes based on the current user, like another concatenated script with
/// permission checked parts. When null, the content is considered not to vary.</param>
public class ConcatenatedScriptPart(Func<string> getScript, Func<bool>? hasPermission = null,
    Func<string?>? getCacheSuffix = null)
{
    /// <summary>
    /// Returns the content of the part
    /// </summary>
    public Func<string> GetScript { get; } = getScript ?? throw new ArgumentNullException(nameof(getScript));

    /// <summary>
    /// Returns true if the current user can access this part. When null, the part is always included.
    /// </summary>
    public Func<bool>? HasPermission { get; } = hasPermission;

    /// <summary>
    /// Returns the cache suffix of the part when its content varies, e.g. based on the current user.
    /// </summary>
    public Func<string?>? GetCacheSuffix { get; } = getCacheSuffix;
}
