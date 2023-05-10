
namespace Serenity.Web;

/// <summary>
/// Abstraction to get cache suffix used while generating 
/// key for a cached dynamic script
/// </summary>
public interface ICacheSuffix
{
    /// <summary>
    /// Gets cache suffix
    /// </summary>
    string CacheSuffix { get; }
}