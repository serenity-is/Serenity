
namespace Serenity.Services;

/// <summary>
/// Service object for delete requests
/// </summary>
public class DeleteRequest : ServiceRequest
{
    /// <summary>
    /// The ID of the entity to delete.
    /// </summary>
    public object EntityId;
}