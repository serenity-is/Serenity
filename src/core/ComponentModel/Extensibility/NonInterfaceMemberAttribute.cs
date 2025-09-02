namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that a property or method should not be included in the automatically generated 
/// interface when using GenerateInterfaceAttribute.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class NonInterfaceMemberAttribute : Attribute
{
}