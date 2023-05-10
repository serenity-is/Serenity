
namespace Serenity.Services;

/// <summary>
/// The response model for a Save service
/// </summary>
public class SaveResponse : ServiceResponse
{
    /// <summary>
    /// The entity ID of the created / updated entity.
    /// </summary>
    public object EntityId;
}