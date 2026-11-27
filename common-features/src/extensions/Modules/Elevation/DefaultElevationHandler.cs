using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Serenity.Extensions;

/// <summary>
/// Default implementation of <see cref="IElevationHandler"/>
/// </summary>
/// <param name="context">The request context.</param>
/// <param name="httpContextAccessor">Gives the request, with the cookie of the token.</param>
/// <param name="dataProtectionProvider">Protects the token.</param>
/// <param name="systemClock">Gives the time.</param>
/// <param name="userRetriever">Gives the current user, to tell when the user has no password to confirm, see
/// <see cref="NoPasswordArgument"/>.</param>
public class DefaultElevationHandler(IRequestContext context, IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider dataProtectionProvider, TimeProvider? systemClock = null,
    IUserRetrieveService? userRetriever = null) : BaseRequestHandler(context), IElevationHandler
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

    /// <summary>
    /// The arguments of the <see cref="RequiresElevationCode"/> error for a user who has no password to confirm, e.g. one
    /// who signs in with an external provider, whom a page asks to set a password instead.
    /// </summary>
    public const string NoPasswordArgument = "NoPassword";

    /// <summary>
    /// The error of a missing, expired or invalid token, which tells when the user has no password to confirm.
    /// </summary>
    private ValidationError Required(string message)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var noPassword = user != null && userRetriever?.GetUserDefinition(user) is IHasPassword { HasPassword: false };
        return new ValidationError(RequiresElevationCode, noPassword ? NoPasswordArgument : null, message);
    }

    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly IDataProtectionProvider dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));

    private DateTimeOffset UtcNow => systemClock?.GetUtcNow() ?? DateTime.UtcNow;

    /// <inheritdoc />
    public void AppendElevationTokenToCookies()
    {
        // an impersonator doesn't confirm the password of the impersonated user
        httpContextAccessor.HttpContext?.User.ValidateNotImpersonating(Localizer);
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
        // the sensitive actions are not available while impersonating
        httpContextAccessor.HttpContext?.User.ValidateNotImpersonating(Localizer);
        if (httpContextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));

        var token = (httpContextAccessor.HttpContext?.Request.Cookies["ElevationToken"]) ??
            throw Required("Token not found");
        var currentUserId = httpContextAccessor.HttpContext.User.GetIdentifier() ??
            throw new ArgumentNullException(nameof(User));

        string userId;
        try
        {
            using var br = dataProtectionProvider.CreateProtector(ElevationPurpose)
                .UnprotectBinary(token);

            var dt = DateTime.SpecifyKind(DateTime.FromBinary(br.ReadInt64()), DateTimeKind.Utc);
            if (dt < UtcNow)
                throw Required("Token expired");

            userId = br.ReadString();
        }
        catch (ValidationError)
        {
            throw;
        }
        catch (Exception)
        {
            throw Required("Invalid elevation token");
        }

        if (!string.Equals(currentUserId, userId, StringComparison.Ordinal))
            throw Required("Invalid elevation token");
    }

    /// <inheritdoc />
    public void DeleteToken()
    {
        if (httpContextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));

        httpContextAccessor.HttpContext.Response.Cookies.Delete("ElevationToken");
    }
}