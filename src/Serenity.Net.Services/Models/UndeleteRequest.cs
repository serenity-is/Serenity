
namespace Serenity.Services;

/// <summary>
/// The request model for an undelete service
/// </summary>
public class UndeleteRequest : ServiceRequest
{
    /// <summary>
    /// The entity ID to undelete
    /// </summary>
    public object EntityId;
}