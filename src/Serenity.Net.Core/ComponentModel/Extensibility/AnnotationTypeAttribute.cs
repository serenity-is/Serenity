namespace Serenity.ComponentModel;

/// <summary>
/// Declares that the type with this attribute is an annotation type for another type,
/// e.g. it contains attributes for the other type and its properties. This is mainly used to
/// separate dependencies between rows and UI related properties like editor types etc.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AnnotationTypeAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of AnnotationType attribute
    /// </summary>
    /// <param name="type">The type to match. It can be a type, an interface 
    /// or an attribute type that type other type should have.</param>
    public AnnotationTypeAttribute(Type type)
    {
        AnnotatedType = type ?? throw new ArgumentNullException("type");
        Inherited = true;
    }

    /// <summary>
    /// Other type, interface or type of attribute that other type should have
    /// </summary>
    public Type AnnotatedType { get; private set; }

    /// <summary>
    /// Should type exactly match, or can it be a subclass of the type.
    /// Default is true, ignored for attribute types.
    /// </summary>
    public bool Inherited { get; set; }

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