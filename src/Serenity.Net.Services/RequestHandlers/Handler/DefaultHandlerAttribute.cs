namespace Serenity.Services;

/// <summary>
/// Specifies that the class this attribute attached to is the default handler (list, create, delete, update etc).
/// and should be used by some behaviors like MasterDetailRelationBehavior instead of creating a generic handler.
/// </summary>
/// <remarks>
/// Creates an instance of the attribute
/// </remarks>
/// <param name="isDefault">True to specify this as default.
/// Default is true.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DefaultHandlerAttribute(bool isDefault = true) : Attribute
{

    /// <summary>
    /// Gets if the handler marked as default.
    /// </summary>
    public bool Value { get; } = isDefault;
}