namespace Serenity.Services;

/// <summary>
/// The response model for an undelete service
/// </summary>
public class UndeleteResponse : ServiceResponse
{
    /// <summary>
    /// True if the entity was not deleted
    /// </summary>
    public bool WasNotDeleted { get; set; }
}