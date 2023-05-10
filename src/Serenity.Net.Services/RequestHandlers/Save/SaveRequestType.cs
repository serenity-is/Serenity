
namespace Serenity.Services;

/// <summary>
/// Save request type for save request processors
/// </summary>
public enum SaveRequestType
{
    /// <summary>
    /// Create
    /// </summary>
    Create,
    /// <summary>
    /// Update
    /// </summary>
    Update,
    /// <summary>
    /// Auto determine from the request.EntityId parameter
    /// </summary>
    Auto
}