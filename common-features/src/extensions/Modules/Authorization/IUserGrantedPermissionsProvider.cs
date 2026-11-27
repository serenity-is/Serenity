namespace Serenity.Extensions;

/// <summary>
/// Gives the permissions that a user is granted, from the roles and the user's own grants, without the revoked ones, so
/// that <see cref="IPermissionGrantValidator"/> can tell whether the current user could have granted them all before
/// changing the account of the user, e.g. its password. The application implements it with its own tables.
/// </summary>
public interface IUserGrantedPermissionsProvider
{
    /// <summary>
    /// Gets the permission keys that a user is granted.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The permission keys.</returns>
    IEnumerable<string> GetGrantedPermissions(string userId);
}
