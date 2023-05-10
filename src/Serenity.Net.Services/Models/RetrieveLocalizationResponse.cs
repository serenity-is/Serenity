namespace Serenity.Services;

/// <summary>
/// The service model for a localization retrieve response
/// </summary>
/// <typeparam name="TEntity">Type of the entities</typeparam>
public class RetrieveLocalizationResponse<TEntity> : ServiceResponse
    where TEntity : class, new()
{
    /// <summary>
    /// The dictionary that contains languageID, localized entity
    /// pairs containing translations.
    /// </summary>
    public Dictionary<string, TEntity> Entities { get; set; }
}