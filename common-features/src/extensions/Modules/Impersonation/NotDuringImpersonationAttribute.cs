namespace Serenity.Extensions;

/// <summary>
/// Marks the actions, or the controllers, that are not available while the user is impersonated, e.g. the ones on the
/// security of the account, like the password or the two-factor authentication, which the filter of Pro's
/// <c>AddImpersonation</c> refuses.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public sealed class NotDuringImpersonationAttribute : Attribute
{
}
