namespace Serenity.TypeScript;

internal class ImportEqualsDeclaration(NodeArray<IModifierLike> modifiers, bool isTypeOnly, string name,
    IModuleReference moduleReference) : DeclarationStatement(SyntaxKind.ImportEqualsDeclaration, name)
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsTypeOnly { get; } = isTypeOnly;
    public IModuleReference ModuleReference { get; set; } = moduleReference;
}

internal class ImportClause(bool isTypeOnly, Identifier name, INamedImportBindings namedBindings)
    : NamedDeclaration(SyntaxKind.ImportClause, name)
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public INamedImportBindings NamedBindings { get; } = namedBindings;
}

internal class NamespaceImport(Identifier name)
    : NamedDeclaration(SyntaxKind.NamespaceImport, name), INamedImportBindings
{
}
