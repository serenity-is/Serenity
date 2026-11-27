namespace Serenity.Extensions;

/// <summary>
/// The request of the permissions that the current user can't grant or revoke.
/// </summary>
public class PermissionGrantListRequest : ServiceRequest
{
    /// <summary>
    /// The permission keys to check, e.g. all the ones of the application.
    /// </summary>
    public List<string>? PermissionKeys { get; set; }
}
