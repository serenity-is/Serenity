namespace Serenity.Abstractions;

/// <summary>
/// Permission service abstraction
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Returns true if user has specified permission
    /// </summary>
    /// <param name="permission">The permission key (e.g. Administration)</param>
    bool HasPermission(string permission);
}