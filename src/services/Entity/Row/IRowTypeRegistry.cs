namespace Serenity.Data;

/// <summary>
/// Abstraction for row type registry.
/// </summary>
public interface IRowTypeRegistry
{
    /// <summary>
    /// Gets all row types.
    /// </summary>
    /// <value>
    /// All row types.
    /// </value>
    IEnumerable<Type> AllRowTypes { get; }
    /// <summary>
    /// Returns row types by the connection key.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>Row types by the connection key</returns>
    IEnumerable<Type> ByConnectionKey(string connectionKey);
}