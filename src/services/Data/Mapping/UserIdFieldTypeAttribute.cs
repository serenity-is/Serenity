namespace Serenity.Data.Mapping;

/// <summary>
/// Declares that the field generated for this property should
/// be of IDFieldType specified <see cref="UserEntityOptions"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class UserIdFieldTypeAttribute() : Attribute
{
}