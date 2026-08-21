namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for a local text registry initializer, which initializes a local text registry with translations.
/// </summary>
public interface ILocalTextInitializer
{
    /// <summary>
    /// Initializes the local text registry with translations.
    /// </summary>
    /// <param name="registry">The target registry.</param>
    void Initialize(ILocalTextRegistry registry);
}