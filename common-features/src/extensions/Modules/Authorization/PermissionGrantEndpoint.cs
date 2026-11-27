namespace Serenity.Extensions.Endpoints;

/// <summary>
/// Tells which permissions the current user can't grant or revoke, by the <see cref="PermissionGrantOptions"/>, so that the
/// pages that manage permissions show them as read-only; the services that save them check them anyway. It tells only about
/// the current user.
/// </summary>
[Route("Services/Extensions/PermissionGrant/[action]")]
[ServiceAuthorize]
public class PermissionGrantEndpoint : ServiceEndpoint
{
    /// <summary>
    /// Lists the permissions, among the given ones, that the current user can't grant or revoke.
    /// </summary>
    /// <param name="request">The request with the permission keys.</param>
    /// <param name="validator">The validator of the grants.</param>
    /// <returns>The permissions that are denied, with the reason.</returns>
    [HttpPost]
    public PermissionGrantListResponse List(PermissionGrantListRequest request,
        [FromServices] IPermissionGrantValidator validator)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(validator);

        var response = new PermissionGrantListResponse
        {
            IsSuperAdmin = validator.IsSuperAdmin,
            Denied = new(StringComparer.OrdinalIgnoreCase)
        };

        if (!response.IsSuperAdmin)
            foreach (var key in (request.PermissionKeys ?? []).Distinct(StringComparer.OrdinalIgnoreCase))
                if (validator.GetDenial(key) is PermissionGrantDenial denial)
                    response.Denied[key] = denial.ToString();

        return response;
    }
}
