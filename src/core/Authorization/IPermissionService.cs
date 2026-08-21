namespace Serenity.Abstractions;

/// <summary>
/// Checks whether the current user has a given permission.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Determines whether the current user has the specified permission.
    /// </summary>
    /// <param name="permission">The permission key to check, for example <c>Administration:General</c>.</param>
    /// <returns><c>true</c> if the user has the permission; otherwise <c>false</c>.</returns>
    bool HasPermission(string permission);
}