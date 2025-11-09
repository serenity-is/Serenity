namespace Serenity.ComponentModel;

/// <summary>
/// Skips a property while generating grid column, form field, or report form parameter list.
/// Use this to ignore a property for UI, but still use it for other 
/// purposes like JSON serialization.
/// This might be useful for example when a type is used as a Service Request and 
/// Form Declaration at the same time. Unlike TransformIgnoreAttribute, this will generate
/// the property in request types, but skip it in UI generation.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class IgnoreUIFieldAttribute : Attribute
{
}