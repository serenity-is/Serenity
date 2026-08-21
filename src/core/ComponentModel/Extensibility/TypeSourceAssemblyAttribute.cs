namespace Serenity.ComponentModel;

/// <summary>
/// Marks an assembly as a source of types for <see cref="ITypeSource"/>.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class TypeSourceAssemblyAttribute() : Attribute
{
}