namespace Serenity.Net.Core.Authorization;

/// <summary>
/// Role Permission Service abstraction 
/// </summary>
public interface IRolePermissionService
{
    /// <summary>
    /// Returns permission keys for give role key
    /// </summary>
    /// <param name="roleKey">Role Key</param>
    /// <returns></returns>
    ISet<string> GetRolePermissions(string roleKey);
}