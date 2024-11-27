namespace Serenity.Data.Mapping;

/// <summary>
/// Indicates if the field is audited. For DataAuditLog, all fields are audited by default,
/// but if the field has [Audited(false)] it will not be logged.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AuditedAttribute(bool value = true) : Attribute
{
    /// <summary>
    /// Gets the value of the attribute
    /// </summary>
    public bool Value { get; } = value;
}