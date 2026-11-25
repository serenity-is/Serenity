namespace Serenity.Localization;

/// <summary>
/// Passes the texts that are added for the invariant language to another registry for a given language instead, e.g.
/// the texts that an <see cref="IImplicitLocalTextRegistrar"/> adds for a type whose assembly is written in another
/// language than the base one (see <see cref="LocalTextLanguageAttribute"/>).
/// </summary>
/// <param name="registry">The registry that the texts are added to.</param>
/// <param name="targetLanguageID">The language of the texts that are added for the invariant language.</param>
internal sealed class InvariantToLanguageRegistry(ILocalTextRegistry registry, string targetLanguageID) : ILocalTextRegistry
{
    public void Add(string languageID, string key, string? text)
    {
        registry.Add(languageID == LocalText.InvariantLanguageID ? targetLanguageID : languageID, key, text);
    }

    public string? TryGet(string languageID, string key, bool pending)
    {
        return registry.TryGet(languageID, key, pending);
    }
}
