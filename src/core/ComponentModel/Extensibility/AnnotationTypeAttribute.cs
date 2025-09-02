namespace Serenity.ComponentModel;

/// <summary>
/// Declares that the type with this attribute is an annotation type for another type,
/// e.g. it contains attributes for the other type and its properties. This is mainly used to
/// separate dependencies between rows and UI related properties like editor types etc.
/// </summary>
/// <remarks>
/// Creates an instance of AnnotationType attribute
/// </remarks>
/// <param name="type">The type to match. It can be a type, an interface 
/// or an attribute type that type other type should have.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AnnotationTypeAttribute(Type type) : Attribute
{

    /// <summary>
    /// Other type, interface or type of attribute that other type should have
    /// </summary>
    public Type AnnotatedType { get; private set; } = type ?? throw new ArgumentNullException("type");

    /// <summary>
    /// Should type exactly match, or can it be a subclass of the type.
    /// Default is true, ignored for attribute types.
    /// </summary>
    public bool Inherited { get; set; } = true;

    /// <summary>
    /// If specified this annotation only applies to types in the namespaces 
    /// and their sub namespaces if namespace ends with ".*"
    /// </summary>
    public string[]? Namespaces { get; set; }

    /// <summary>
    /// If specified, this annotation matches the type only if it has all the
    /// properties listed in this array.
    /// </summary>
    public string[]? Properties { get; set; }
}