namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for local text registry initializer, which initializes local text registry with translations.
/// </summary>
public interface ILocalTextInitializer
{
    /// <summary>
    /// Initializes local text registry with translations.
    /// </summary>
    /// <param name="registry">The target registry</param>
    void Initialize(ILocalTextRegistry registry);
}