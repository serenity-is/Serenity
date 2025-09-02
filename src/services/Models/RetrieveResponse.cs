using System.Collections;

namespace Serenity.Services;

/// <summary>
/// The service response model for a Retrieve service
/// </summary>
/// <typeparam name="T">Type of the entity</typeparam>
public class RetrieveResponse<T> : ServiceResponse, IRetrieveResponse
{
    /// <summary>
    /// The returned entity
    /// </summary>
    public T Entity { get; set; }

    /// <summary>
    /// Dictionary containing localizations if requested.
    /// </summary>
    public Dictionary<string, T> Localizations { get; set; }

    object IRetrieveResponse.Entity => Entity;

    IDictionary IRetrieveResponse.Localizations
    {
        get { return Localizations; }
        set { Localizations = (Dictionary<string, T>)value; }
    }
}