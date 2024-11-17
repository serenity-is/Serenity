namespace Serenity.Data;

/// <summary>
/// Declares that the property generated for this template property should
/// have the specified name, instead of the auto generated one.
/// </summary>
/// <remarks>
/// Creates an instance of EmitNameAttribute attribute
/// </remarks>
/// <param name="name">Property name to use for this property 
/// in generated source.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class EmitNameAttribute(string name) : Attribute
{

    /// <summary>
    /// The property name
    /// </summary>
    public string Name { get; private set; } = name ?? throw new ArgumentNullException(nameof(name));
}