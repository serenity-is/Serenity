namespace Serenity.Abstractions;

/// <summary>
/// Checks whether a specific role has a given permission.
/// </summary>
public interface IRolePermissionService
{
    /// <summary>
    /// Determines whether the specified role has the given permission.
    /// </summary>
    /// <param name="role">The role key or name.</param>
    /// <param name="permission">The permission key to check.</param>
    /// <returns><c>true</c> if the role has the permission; otherwise <c>false</c>.</returns>
    bool HasPermission(string role, string permission);
}