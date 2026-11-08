namespace Serenity.ComponentModel;

/// <summary>
/// Skips a property while generating a grid column, form field, or report form parameter list.
/// Use this to ignore a property for UI, but still use it for other
/// purposes like JSON serialization.
/// This might be useful for example when a type is used as a Service Request and
/// Form Declaration at the same time. Unlike TransformIgnoreAttribute, this will generate
/// the property in request types, but skip it in UI generation.
/// <para>
/// It can also be placed on the members of a <see cref="StringEnum"/>, like
/// <see cref="TransformIgnoreAttribute"/>, to keep them out of the generated type on the client. Such members are
/// still valid values on the server.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class IgnoreUIFieldAttribute : Attribute
{
}