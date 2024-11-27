using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Serenity.Reporting;

/// <summary>
/// Implementation for <see cref="IReportExecutor" /> that uses callback report cookie
/// to impersonate / transient grant permissions
/// </summary>
public class HtmlReportCallbackUrlInterceptor(
    ILogger<HtmlReportCallbackUrlBuilder> logger,
    IPermissionService permissionService = null,
    IUserAccessor userAccessor = null,
    IUserClaimCreator userClaimCreator = null,
    IHttpContextAccessor httpContextAccessor = null,
    IDataProtectionProvider dataProtectionProvider = null) : IReportCallbackInterceptor
{
    public ReportRenderResult InterceptCallback(ReportRenderOptions options, Func<ReportRenderOptions, ReportRenderResult> action)
    {
        IImpersonator impersonator = userAccessor as IImpersonator;
        ITransientGrantor transientGrantor = permissionService as ITransientGrantor;
        bool undoImpersonate = false;
        bool undoGrant = false;
        try
        {
            try
            {
                if (dataProtectionProvider != null &&
                    (impersonator != null || transientGrantor != null) &&
                    httpContextAccessor?.HttpContext?.Request?.Cookies?.TryGetValue(
                    HtmlReportCallbackUrlBuilder.ReportAuthCookieName, out var token) == true &&
                    !string.IsNullOrEmpty(token))
                {
                    using var br = dataProtectionProvider.CreateProtector(HtmlReportCallbackUrlBuilder.ReportAuthCookieName)
                        .UnprotectBinary(token);
                    var dt = DateTime.FromBinary(br.ReadInt64());
                    if (dt > DateTime.UtcNow)
                    {
                        var username = br.ReadString();
                        if (impersonator != null &&
                            !string.IsNullOrEmpty(username) &&
                            userClaimCreator != null &&
                            userAccessor?.User?.Identity?.Name != username)
                        {
                            var principal = userClaimCreator.CreatePrincipal(username, "ReportImpersonation");
                            impersonator.Impersonate(principal);
                            undoImpersonate = true;
                        }

                        if (transientGrantor != null)
                        {
                            var count = br.ReadInt32();
                            if (count == -1)
                            {
                                transientGrantor.GrantAll();
                                undoGrant = true;
                            }
                            else if (count > 0 && count < 10000)
                            {
                                var perms = new string[count];
                                for (var i = 0; i < count; i++)
                                    perms[i] = br.ReadString();
                                transientGrantor.Grant(perms);
                                undoGrant = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ignore errors while decrypting / deserializing / applying ticket
                logger.LogError(ex, "Error decrypting/applying report auth ticket");
            }

            return action(options);
        }
        finally
        {
            if (undoImpersonate)
                impersonator.UndoImpersonate();
            if (undoGrant)
                transientGrantor.UndoGrant();
        }
    }
}