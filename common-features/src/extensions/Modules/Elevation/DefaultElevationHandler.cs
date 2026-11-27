using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Serenity.Extensions;

/// <summary>
/// Default implementation of <see cref="IElevationHandler"/>
/// </summary>
public class DefaultElevationHandler(IRequestContext context, IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider dataProtectionProvider, TimeProvider? systemClock = null) : BaseRequestHandler(context), IElevationHandler
{
    private const string ElevationPurpose = "Elevation";

    /// <summary>
    /// The duration in minutes that an elevation token remains valid.
    /// </summary>
    public const int ElevationTokenDuration = 15;

    /// <summary>
    /// The error code of the validation error for a missing, expired or invalid elevation token, which a page handles by
    /// asking for the password, and calling the service again.
    /// </summary>
    public const string RequiresElevationCode = "RequiresElevation";

    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly IDataProtectionProvider dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));

    private DateTimeOffset UtcNow => systemClock?.GetUtcNow() ?? DateTime.UtcNow;

    /// <inheritdoc />
    public void AppendElevationTokenToCookies()
    {
        if (httpContextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));

        var userId = httpContextAccessor.HttpContext.User.GetIdentifier() ?? throw new ArgumentNullException(nameof(User));
        var token = dataProtectionProvider.CreateProtector(ElevationPurpose)
            .ProtectBinary(bw =>
            {
                bw.Write(UtcNow.AddMinutes(ElevationTokenDuration).DateTime.ToBinary());
                bw.Write(userId);
            });

        httpContextAccessor.HttpContext.Response.Cookies.Append("ElevationToken", token, new CookieOptions()
        {
            SameSite = SameSiteMode.Lax,
            Secure = true,
            HttpOnly = true,
            Expires = UtcNow.AddMinutes(ElevationTokenDuration)
        });
    }

    /// <inheritdoc />
    public void ValidateElevationToken()
    {
        if (httpContextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));

        var token = (httpContextAccessor.HttpContext?.Request.Cookies["ElevationToken"]) ??
            throw new ValidationError(RequiresElevationCode, "Token not found");
        var currentUserId = httpContextAccessor.HttpContext.User.GetIdentifier() ??
            throw new ArgumentNullException(nameof(User));

        string userId;
        try
        {
            using var br = dataProtectionProvider.CreateProtector(ElevationPurpose)
                .UnprotectBinary(token);

            var dt = DateTime.SpecifyKind(DateTime.FromBinary(br.ReadInt64()), DateTimeKind.Utc);
            if (dt < UtcNow)
                throw new ValidationError(RequiresElevationCode, "Token expired");

            userId = br.ReadString();
        }
        catch (ValidationError)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ValidationError(RequiresElevationCode, "Invalid elevation token");
        }

        if (!string.Equals(currentUserId, userId, StringComparison.Ordinal))
            throw new ValidationError(RequiresElevationCode, "Invalid elevation token");
    }

    /// <inheritdoc />
    public void DeleteToken()
    {
        if (httpContextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));

        httpContextAccessor.HttpContext.Response.Cookies.Delete("ElevationToken");
    }
}