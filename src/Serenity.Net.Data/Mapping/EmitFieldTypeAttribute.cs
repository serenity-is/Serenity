namespace Serenity.Data;

/// <summary>
/// Declares that the field generated for this template property should
/// be of type specified.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class EmitFieldTypeAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of FieldTypeAttribute attribute
    /// </summary>
    /// <param name="fieldType">Field type to use for this property 
    /// in generated source.</param>
    public EmitFieldTypeAttribute(Type fieldType)
    {
        FieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
    }

    /// <summary>
    /// The field type
    /// </summary>
    public Type FieldType { get; private set; }
}