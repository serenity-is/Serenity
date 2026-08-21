namespace Serenity.Extensions;

/// <summary>
/// The request model for updating a user preference.
/// </summary>
public class UserPreferenceUpdateRequest : ServiceRequest
{
    /// <summary>
    /// The preference type.
    /// </summary>
    public string PreferenceType { get; set; }
    /// <summary>
    /// The preference name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The preference value.
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// The request model for retrieving a user preference.
/// </summary>
public class UserPreferenceRetrieveRequest : ServiceRequest
{
    /// <summary>
    /// The preference type.
    /// </summary>
    public string PreferenceType { get; set; }
    /// <summary>
    /// The preference name.
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// The response model for retrieving a user preference.
/// </summary>
public class UserPreferenceRetrieveResponse : ServiceResponse
{
    /// <summary>
    /// The preference value.
    /// </summary>
    public string Value { get; set; }
}