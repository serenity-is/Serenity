namespace Serenity.Services;

/// <summary>
/// The base service response object model for all service response
/// types. Your custom response objects should derive from this class.
/// </summary>
public class ServiceResponse
{
    /// <summary>
    /// The error returned if any.
    /// </summary>
    public ServiceError Error { get; set; }

    /// <summary>
    /// A custom data dictionary. Please consider subclassing the service response
    /// before passing data via this property. This should be only used in limited 
    /// cases where subclassing is not feasible.
    /// Another option could be to add a [JsonExtensionData] but that could open
    /// way to the side effect ignoring typos.
    /// </summary>
    public Dictionary<string, object> CustomData { get; set; }
}