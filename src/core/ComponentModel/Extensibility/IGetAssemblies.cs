namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for type source that can return a list of assemblies
/// </summary>
public interface IGetAssemblies
{
    /// <summary>
    /// Gets all distinct assemblies in the type source
    /// </summary>
    IEnumerable<Assembly> GetAssemblies();
}