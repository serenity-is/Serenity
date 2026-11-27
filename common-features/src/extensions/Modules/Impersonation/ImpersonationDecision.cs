namespace Serenity.Extensions;

/// <summary>
/// What an <see cref="IImpersonationPolicy"/> says about a user impersonating another one.
/// </summary>
public enum ImpersonationDecision
{
    /// <summary>The policy has no say, e.g. the impersonator doesn't have its permission.</summary>
    Abstain = 0,

    /// <summary>The policy allows it, unless another one denies it.</summary>
    Allow = 1,

    /// <summary>The policy denies it, whatever the others say.</summary>
    Deny = 2
}
