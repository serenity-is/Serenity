namespace Serenity.TypeScript;

public class NamespaceExport(IModuleExportName name)
    : NamedDeclaration<IModuleExportName>(SyntaxKind.NamespaceExport, name), INamedExportBindings
{
}