
namespace Serenity.Abstractions;

/// <summary>
/// Interface for permission services that supports granting permissions temporarily
/// </summary>
public interface ITransientGrantor
{
    /// <summary>
    /// Grants specified permissions temporarily (or makes it look like)
    /// </summary>
    /// <param name="permissions">List of permission keys</param>
    void Grant(params string[] permissions);
    /// <summary>
    /// Grants all permissions temporarily (or makes it look like)
    /// </summary>
    void GrantAll();
    /// <summary>
    /// Undoes last grant or grant all operation
    /// </summary>
    void UndoGrant();
}