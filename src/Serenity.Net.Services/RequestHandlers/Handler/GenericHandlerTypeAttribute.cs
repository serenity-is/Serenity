namespace Serenity.Services;

/// <summary>
/// Assigns the generic handler type (e.g. <see cref="SaveRequestHandler{TRow}"/> 
/// for a handler interface (like <see cref="ISaveRequestHandler"/>) 
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class GenericHandlerTypeAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of the attribute
    /// </summary>
    /// <param name="type">The handler type</param>
    public GenericHandlerTypeAttribute(Type type)
    {
        Value = type ?? throw new ArgumentNullException("type");
    }

    /// <summary>
    /// The generic handler type.
    /// </summary>
    public Type Value { get; }
}