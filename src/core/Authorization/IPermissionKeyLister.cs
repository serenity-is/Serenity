namespace Serenity.Abstractions;

/// <summary>
/// Permission key lister abstraction
/// </summary>
public interface IPermissionKeyLister
{
    /// <summary>
    /// List all permission keys
    /// </summary>
    /// <param name="includeRoles">True to include role permission keys like Role:Some</param>
    IEnumerable<string> ListPermissionKeys(bool includeRoles);
}