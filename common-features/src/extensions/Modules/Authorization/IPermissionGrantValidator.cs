namespace Serenity.Extensions;

/// <summary>
/// Checks whether the current user can grant or revoke permissions, by the <see cref="PermissionGrantOptions"/>, and can
/// change the account of a user, which only someone who could have granted all the permissions of the user can, so an
/// administrator can't take over the account of someone with more rights, e.g. by setting its password.
/// </summary>
public interface IPermissionGrantValidator
{
    /// <summary>
    /// Whether the current user is a super admin, who can grant anything and change any account.
    /// </summary>
    bool IsSuperAdmin { get; }

    /// <summary>
    /// Gets why the current user can't grant or revoke a permission, also checking the permissions that it implies, or
    /// null when the user can.
    /// </summary>
    /// <param name="permissionKey">The permission key.</param>
    /// <returns>The reason, or null.</returns>
    PermissionGrantDenial? GetDenial(string permissionKey);

    /// <summary>
    /// Throws a validation error when the current user can't grant or revoke one of the permissions.
    /// </summary>
    /// <param name="permissionKeys">The permissions that change, e.g. the ones granted to a role and the ones removed.</param>
    void ValidateGrants(IEnumerable<string> permissionKeys);

    /// <summary>
    /// Throws a validation error when one of the permissions is not one that the application lists with its
    /// <see cref="IPermissionKeyLister"/>, e.g. a private one like "ImpersonateAs" that the permission service decides by
    /// itself, a marker like "*", a role key like "Role:Administrators", which would give the permissions of a role without
    /// being a member, or a typo. It applies to super admins too. Only the permissions that are added need it, so ones that
    /// are no longer listed, e.g. of a removed module, can still be removed.
    /// </summary>
    /// <param name="permissionKeys">The permissions that are added, e.g. to a role.</param>
    void ValidateListed(IEnumerable<string> permissionKeys);

    /// <summary>
    /// Gets why the current user can't change the account of a user, e.g. its password, two-factor authentication,
    /// username, email or status, or null when the user can: the account of the current user, or of a user whose
    /// permissions the current user could have granted all.
    /// </summary>
    /// <param name="user">The user whose account it is.</param>
    /// <returns>The reason, or null.</returns>
    PermissionGrantDenial? GetUserDenial(IUserDefinition user);

    /// <summary>
    /// Throws a validation error when the current user can't change the account of a user.
    /// </summary>
    /// <param name="user">The user whose account it is.</param>
    void ValidateUser(IUserDefinition user);
}
