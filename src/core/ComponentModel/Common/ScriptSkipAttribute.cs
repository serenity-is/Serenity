namespace Serenity.ComponentModel;

/// <summary>
/// Disables script code generation for the type this attribute is placed on.
/// Currently only useful for LocalText/Permission generation.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class ScriptSkipAttribute : Attribute
{
}