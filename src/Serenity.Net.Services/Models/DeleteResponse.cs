
namespace Serenity.Services;

/// <summary>
/// The response object returned from a delete service
/// </summary>
public class DeleteResponse : ServiceResponse
{
    /// <summary>
    /// True if the entity was already deleted.
    /// Not all services support this. Only soft delete
    /// services may return this information, while 
    /// others may raise a record not found exception.
    /// </summary>
    public bool WasAlreadyDeleted;
}