namespace Serenity.Services;

/// <summary>
/// Specifies that the class this attribute attached to is the default handler (list, create, delete, update etc).
/// and should be used by some behaviors like MasterDetailRelationBehavior instead of creating a generic handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DefaultHandlerAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of the attribute
    /// </summary>
    /// <param name="isDefault">True to specify this as default.
    /// Default is true.</param>
    public DefaultHandlerAttribute(bool isDefault = true)
    {
        Value = isDefault;
    }

    /// <summary>
    /// Gets if the handler marked as default.
    /// </summary>
    public bool Value { get; }
}