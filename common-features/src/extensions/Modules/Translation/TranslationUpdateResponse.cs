namespace Serenity.Extensions;

/// <summary>
/// The response model for a translation update request.
/// </summary>
public class TranslationUpdateResponse : ServiceResponse
{
    /// <summary>
    /// The path where the translations were saved.
    /// </summary>
    public string SavedPath { get; set; }
}