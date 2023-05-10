namespace Serenity.Abstractions;

/// <summary>
/// Interface for authorization services that supports temporary impersonating
/// </summary>
public interface IImpersonator
{
    /// <summary>
    /// Temporarily impersonates as a user
    /// </summary>
    /// <param name="user">User to impersonate as</param>
    void Impersonate(ClaimsPrincipal user);

    /// <summary>
    /// Undoes impersonation
    /// </summary>
    void UndoImpersonate();
}