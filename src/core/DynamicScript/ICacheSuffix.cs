
namespace Serenity.Web;

/// <summary>
/// Abstraction to get the cache suffix used while generating the key
/// for a cached dynamic script.
/// </summary>
public interface ICacheSuffix
{
    /// <summary>
    /// Gets the cache suffix.
    /// </summary>
    string CacheSuffix { get; }
}