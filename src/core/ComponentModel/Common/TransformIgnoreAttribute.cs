namespace Serenity.ComponentModel;

/// <summary>
/// Disables script code generation for the type / property this attribute is placed on.
/// If placed on a form / column property it will also skip generating a corresponding column / form field for that property.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
public class TransformIgnoreAttribute : Attribute
{
}