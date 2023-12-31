namespace Serenity.TypeScript;

internal class ClassElement(SyntaxKind kind, IPropertyName name)
    : NamedDeclaration<IPropertyName>(kind, name), IClassElement
{
}