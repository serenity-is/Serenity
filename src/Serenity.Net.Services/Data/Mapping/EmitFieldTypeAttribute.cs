namespace Serenity.Data;

/// <summary>
/// Declares that the field generated for this template property should
/// be of type specified.
/// </summary>
/// <remarks>
/// Creates an instance of FieldTypeAttribute attribute
/// </remarks>
/// <param name="fieldType">Field type to use for this property 
/// in generated source.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class EmitFieldTypeAttribute(Type fieldType) : Attribute
{

    /// <summary>
    /// The field type
    /// </summary>
    public Type FieldType { get; private set; } = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
}