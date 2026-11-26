namespace Serenity.Extensions;

/// <summary>
/// Per path overrides for <see cref="AccountRateLimitOptions.Paths"/>. Null values fall back
/// to <see cref="AccountRateLimitOptions.PermitLimit"/> and <see cref="AccountRateLimitOptions.WindowSeconds"/>.
/// </summary>
public class AccountRateLimitPathOptions
{
    /// <summary>
    /// Maximum number of requests allowed per client IP address within the window for this path.
    /// Set to 0 to disable rate limiting for this path.
    /// </summary>
    public int? PermitLimit { get; set; }

    /// <summary>
    /// Length of the fixed window in seconds for this path.
    /// </summary>
    public int? WindowSeconds { get; set; }
}
