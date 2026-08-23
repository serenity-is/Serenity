namespace Serenity.Data;

/// <summary>
/// Declares a fallback connection key for a logical connection key.
/// When a connection key is not present in configuration, the fallback connection key is used.
/// </summary>
/// <remarks>
/// This attribute can be applied at the assembly level, e.g.
/// <c>[assembly: ConnectionKeyFallback("ProMeeting", "Default")]</c>.
/// When multiple assemblies declare a fallback for the same connection key, the declaration
/// in the assembly that appears later in the type source (typically the application assembly)
/// wins, so applications can override framework defaults.
/// The attribute is not sealed so that feature-specific derived attributes can be created,
/// e.g. <c>ProSqlFileSystemFallbackAttribute : ConnectionKeyFallbackAttribute</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ConnectionKeyFallbackAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionKeyFallbackAttribute"/> class.
    /// </summary>
    /// <param name="connectionKey">The logical connection key.</param>
    /// <param name="fallbackConnectionKey">
    /// The connection key to use when <paramref name="connectionKey"/> is not configured.
    /// </param>
    public ConnectionKeyFallbackAttribute(string connectionKey, string fallbackConnectionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionKey);
        ArgumentException.ThrowIfNullOrEmpty(fallbackConnectionKey);
        ConnectionKey = connectionKey;
        FallbackConnectionKey = fallbackConnectionKey;
    }

    /// <summary>
    /// Gets the logical connection key.
    /// </summary>
    public string ConnectionKey { get; }

    /// <summary>
    /// Gets the fallback connection key used when <see cref="ConnectionKey"/> is not configured.
    /// </summary>
    public string FallbackConnectionKey { get; }
}
