namespace Serenity.Extensions;

/// <summary>
/// Local text keys for account rate limiting.
/// </summary>
[NestedLocalTexts(Prefix = "Validation.")]
public static class AccountRateLimitTexts
{
    public static readonly LocalText TooManyRequests = "Too many attempts. Please wait a while and try again.";
}
