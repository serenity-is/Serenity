namespace Serenity.ComponentModel;

/// <summary>
/// Enables script code generation for the type this attribute is placed on.
/// By default, types which are used in an endpoint / row is automatically
/// enabled for code generation. Use this only for non-referenced types.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false)]
public class ScriptIncludeAttribute : Attribute
{
}