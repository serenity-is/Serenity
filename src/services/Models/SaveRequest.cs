
using System.Collections;

namespace Serenity.Services;

/// <summary>
/// The service model for a Save (Create / Update) service.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SaveRequest<TEntity> : ServiceRequest, ISaveRequest
{
    /// <inheritdoc/>
    public object EntityId { get; set; }
    
    /// <summary>
    /// The entity containing only the fields that should
    /// be inserted / updated. Partial patch is only possible
    /// with Row types as only it provides assignment information
    /// from the originating JSON.
    /// </summary>
    public TEntity Entity { get; set; }

    /// <summary>
    /// The set of localizations if translations are requested to 
    /// be updated.
    /// </summary>
    public Dictionary<string, TEntity> Localizations { get; set; }

    object ISaveRequest.Entity
    {
        get { return Entity; }
        set { Entity = (TEntity)value; }
    }

    IDictionary ISaveRequest.Localizations
    {
        get { return Localizations; }
        set { Localizations = (Dictionary<string, TEntity>)value; }
    }
}
