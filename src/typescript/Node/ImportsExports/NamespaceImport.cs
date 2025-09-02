namespace Serenity.TypeScript;

public class NamespaceImport(Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.NamespaceImport, name), INamedImportBindings
{
}
