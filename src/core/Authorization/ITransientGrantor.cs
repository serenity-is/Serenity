
namespace Serenity.Abstractions;

/// <summary>
/// Defines a permission service that supports temporarily granting permissions.
/// </summary>
public interface ITransientGrantor
{
    /// <summary>
    /// Temporarily grants the specified permissions.
    /// </summary>
    /// <param name="permissions">The permission keys to grant.</param>
    void Grant(params string[] permissions);

    /// <summary>
    /// Temporarily grants all permissions.
    /// </summary>
    void GrantAll();

    /// <summary>
    /// Reverts the most recent <see cref="Grant"/> or <see cref="GrantAll"/> operation.
    /// </summary>
    void UndoGrant();

    /// <summary>
    /// Determines whether all permissions are currently granted via <see cref="GrantAll"/>.
    /// </summary>
    /// <returns><c>true</c> if all permissions are granted; otherwise <c>false</c>.</returns>
    bool IsAllGranted();

    /// <summary>
    /// Gets the permissions that were granted via <see cref="Grant"/>.
    /// </summary>
    /// <returns>An enumerable of granted permission keys.</returns>
    IEnumerable<string> GetGranted();
}