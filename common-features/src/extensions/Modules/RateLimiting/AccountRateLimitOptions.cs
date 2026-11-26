namespace Serenity.Extensions;

/// <summary>
/// Options for the rate limiter registered by
/// <see cref="DependencyInjection.AccountRateLimiterServiceCollectionExtensions.AddAccountRateLimiter"/>,
/// which limits POST requests to account actions like login and forgot password per client IP address.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class AccountRateLimitOptions
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "AccountRateLimit";

    /// <summary>
    /// Whether the rate limiting is enabled. Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Request paths (excluding the path base) that are rate limited for POST requests, with optional
    /// per path overrides. Each path has its own limit, e.g. login attempts do not use up the forgot
    /// password limit. Paths are matched case-insensitively, ignoring a trailing slash.
    /// Configuration is merged into the defaults, so a path can be added or overridden without
    /// repeating the others, e.g. <c>"Paths": { "/Account/SignUp": { "PermitLimit": 5 } }</c>,
    /// and a default path can be disabled with <c>"PermitLimit": 0</c>.
    /// </summary>
    public Dictionary<string, AccountRateLimitPathOptions> Paths { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/Account/Login"] = new(),
        ["/Account/ForgotPassword"] = new(),
        ["/Account/ResetPassword"] = new(),
        ["/Account/SignUp"] = new(),
        ["/Account/Elevate"] = new()
    };

    /// <summary>
    /// Maximum number of requests allowed per client IP address and path within <see cref="WindowSeconds"/>.
    /// Default is 20. Keep in mind that users behind the same proxy or NAT share an IP address.
    /// </summary>
    public int PermitLimit { get; set; } = 20;

    /// <summary>
    /// Length of the fixed window in seconds. Default is 60.
    /// </summary>
    public int WindowSeconds { get; set; } = 60;
}
