namespace Serenity.Services;

/// <summary>
/// Assigns the generic handler type (e.g. <see cref="SaveRequestHandler{TRow}"/>
/// for a handler interface (like <see cref="ISaveRequestHandler"/>).
/// </summary>
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
/// <param name="type">The handler type</param>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class GenericHandlerTypeAttribute(Type type) : Attribute
{

    /// <summary>
    /// Gets the generic handler type.
    /// </summary>
    public Type Value { get; } = type ?? throw new ArgumentNullException("type");
}