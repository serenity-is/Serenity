namespace Serenity.Extensions;

/// <summary>
/// Marks a controller or an action that a user who must still set up the account (see <see cref="IAccountSetupRequirement"/>)
/// can use before it is done, e.g. the pages and services that change the password or set up two-factor authentication,
/// the confirmation of the password, and signing out. <see cref="AccountSetupFilter"/> refuses the other ones until then.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public sealed class AllowDuringAccountSetupAttribute : Attribute
{
}
