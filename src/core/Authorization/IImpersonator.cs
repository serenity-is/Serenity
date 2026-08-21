namespace Serenity.Abstractions;

/// <summary>
/// Defines a service that supports temporary user impersonation.
/// </summary>
public interface IImpersonator
{
    /// <summary>
    /// Temporarily impersonates the specified user.
    /// </summary>
    /// <param name="user">The principal to impersonate.</param>
    void Impersonate(ClaimsPrincipal user);

    /// <summary>
    /// Ends the most recent impersonation and restores the previous principal.
    /// </summary>
    void UndoImpersonate();
}