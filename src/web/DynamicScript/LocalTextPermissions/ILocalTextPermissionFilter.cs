namespace Serenity.Web;

/// <summary>
/// Filters the local texts sent to the client by the permissions of the current user.
/// </summary>
public interface ILocalTextPermissionFilter
{
    /// <summary>
    /// Gets a cache suffix identifying which texts the current user may see. Users with the
    /// same suffix see the same texts. Empty when filtering is not enabled.
    /// </summary>
    string GetCacheSuffix();

    /// <summary>
    /// Gets a predicate that returns true for the local text keys the current user may see,
    /// or null when filtering is not enabled.
    /// </summary>
    Func<string, bool>? GetKeyFilter();
}
