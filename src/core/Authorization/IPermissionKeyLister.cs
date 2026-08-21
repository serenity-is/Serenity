namespace Serenity.Abstractions;

/// <summary>
/// Enumerates all permission keys registered in the application.
/// </summary>
public interface IPermissionKeyLister
{
    /// <summary>
    /// Lists all permission keys.
    /// </summary>
    /// <param name="includeRoles">When <c>true</c>, includes role-derived keys such as <c>Role:SomeRole</c>.</param>
    /// <returns>An enumerable of permission keys.</returns>
    IEnumerable<string> ListPermissionKeys(bool includeRoles);
}