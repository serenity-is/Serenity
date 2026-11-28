namespace Serenity.Extensions;

/// <summary>
/// Gives the permissions that roles grant, so that the ones who give a role can be checked to be able to grant them all,
/// e.g. by <see cref="IInvitationPolicy"/>s for the roles of an invitation. The application implements it with its own
/// tables.
/// </summary>
public interface IRoleGrantedPermissionsProvider
{
    /// <summary>
    /// Gets the permission keys that any of the roles grants.
    /// </summary>
    /// <param name="roleIds">The IDs of the roles, as strings.</param>
    /// <returns>The permission keys.</returns>
    IEnumerable<string> GetGrantedPermissions(IEnumerable<string> roleIds);
}
