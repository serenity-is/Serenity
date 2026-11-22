namespace Serenity.Services;

/// <summary>
/// An interface to provide a text key for localization purposes.
/// </summary>
public interface IHasTextKey
{
    /// <summary>
    /// Gets the text key used for localization purposes. A text key that is empty, i.e. <see cref="Serenity.LocalText.Literal"/>,
    /// means the text is not a local text key: it is neither used as one, nor registered under the key that its type might imply for it, and it is
    /// shown as it is, which is what a text that is a name, e.g. of a company, needs. One that is not set leaves the
    /// decision to the attribute, which usually implies a key, e.g. "Forms.MyForm.CompanyName". As an attribute is the
    /// only way to set a key, an empty one is always a deliberate choice, and it works the same for the members of an
    /// enum, whose texts the client gets with the key and the text that its enum carries.
    /// </summary>
    string? TextKey { get; }
}