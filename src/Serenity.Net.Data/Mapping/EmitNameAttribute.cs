namespace Serenity.Data;

/// <summary>
/// Declares that the property generated for this template property should
/// have the specified name, instead of the auto generated one.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class EmitNameAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of EmitNameAttribute attribute
    /// </summary>
    /// <param name="name">Property name to use for this property 
    /// in generated source.</param>
    public EmitNameAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// The property name
    /// </summary>
    public string Name { get; private set; }
}