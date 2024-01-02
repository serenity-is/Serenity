namespace Serenity.TypeScript;

public class NamespaceExport(Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.NamespaceExport, name), INamedExportBindings
{
}