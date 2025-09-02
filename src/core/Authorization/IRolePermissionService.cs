namespace Serenity.Abstractions;

/// <summary>
/// Role Permission Service abstraction 
/// </summary>
public interface IRolePermissionService
{
    /// <summary>
    /// Return if given role has given permission.
    /// </summary>
    /// <param name="role">Role Key or Name</param>
    /// <param name="permission">Permission Key</param>
    /// <returns></returns>
    bool HasPermission(string role, string permission);
}