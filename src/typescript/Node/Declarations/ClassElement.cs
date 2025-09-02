namespace Serenity.TypeScript;

public class ClassElement(SyntaxKind kind, IPropertyName name)
    : NamedDeclaration<IPropertyName>(kind, name), IClassElement
{
}