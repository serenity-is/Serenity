namespace Serenity.Services;

/// <summary>
/// Specifies that the class this attribute is attached to is the default handler
/// (list, create, delete, update etc.) and should be used by some behaviors like
/// MasterDetailRelationBehavior instead of creating a generic handler.
/// </summary>
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
/// <param name="isDefault">True to specify this as default.
/// Default is true.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DefaultHandlerAttribute(bool isDefault = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether the handler is marked as default.
    /// </summary>
    public bool Value { get; } = isDefault;
}