namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to access the current user
/// </summary>
public interface IUserAccessor
{
    /// <summary>
    /// Gets current user
    /// </summary>
    ClaimsPrincipal? User { get; }
}