namespace Serenity.Data;

/// <summary>
/// Indicates that the entity should be audit logged.
/// This feature is only available in premium app.
/// </summary>
/// <seealso cref="Attribute" />
public class DataAuditLogAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataAuditLogAttribute"/> class.
    /// </summary>
    public DataAuditLogAttribute()
    {
    }
}