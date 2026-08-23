namespace Serenity.Data;

/// <summary>
/// Abstraction to access connection key fallbacks.
/// </summary>
/// <remarks>
/// A connection key fallback maps a logical connection key (e.g. "ProMeeting") to another
/// connection key (e.g. "Default") that should be used when the logical key is not
/// present in configuration. This allows feature modules to declare their own connection
/// key while still working in applications that only configure the fallback key.
/// </remarks>
public interface IConnectionKeyFallbacks
{
    /// <summary>
    /// Gets the ordered fallback chain for a connection key, starting with the key itself
    /// followed by its declared fallbacks. This reflects only the declared
    /// <see cref="ConnectionKeyFallbackAttribute"/> values and does not check configuration.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The ordered fallback chain.</returns>
    IEnumerable<string> GetConnectionKeyFallbacks(string connectionKey);

    /// <summary>
    /// Resolves a connection key to the first key in its fallback chain that is actually
    /// configured. Returns <c>null</c> if none of the keys in the chain are configured.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The resolved connection key, or <c>null</c>.</returns>
    string ResolveConnectionKey(string connectionKey);

    /// <summary>
    /// Returns all connection keys (including themselves) whose fallback chain resolves to
    /// the specified connection key. Only keys that actually resolve to a <em>configured</em>
    /// connection are included, so this is a configuration-aware operation.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The connection keys resolving to the specified key.</returns>
    IEnumerable<string> GetConnectionKeysResolvingTo(string connectionKey);
}
