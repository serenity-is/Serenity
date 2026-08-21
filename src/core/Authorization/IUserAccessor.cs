namespace Serenity.Abstractions;

/// <summary>
/// Provides access to the current authenticated user.
/// </summary>
public interface IUserAccessor
{
    /// <summary>
    /// Gets the current user principal, or <c>null</c> if no user is authenticated.
    /// </summary>
    ClaimsPrincipal? User { get; }
}