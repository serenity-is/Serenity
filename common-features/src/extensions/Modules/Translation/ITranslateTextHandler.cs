namespace Serenity.Extensions;

/// <summary>
/// Interface for handlers that translate texts.
/// </summary>
public interface ITranslateTextHandler
{
    /// <summary>
    /// Translates the texts in the given request.
    /// </summary>
    /// <param name="request">The translation request.</param>
    /// <returns>The translation response.</returns>
    TranslateTextResponse Translate(TranslateTextRequest request);
}