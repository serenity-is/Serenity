namespace Serenity.Abstractions;

/// <summary>
/// Adds local texts that are kept in the data of the application, e.g. the translations of the titles of its dashboards
/// in the menu, to the registry when the texts are initialized, which is when the application starts and after the texts
/// are reset, e.g. when translations are saved. It is called after the base texts and the texts files are added, so what is
/// there can be told apart, and the source only adds what is missing.
/// </summary>
/// <remarks>
/// The initializer is a singleton, so the sources must be too. A source that reads a database should not fail when it is not
/// ready yet, as the texts are initialized when the application starts.
/// </remarks>
public interface IDynamicLocalTextSource
{
    /// <summary>
    /// Adds the texts to the registry.
    /// </summary>
    /// <param name="registry">The registry.</param>
    void AddTexts(ILocalTextRegistry registry);
}
