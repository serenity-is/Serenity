namespace Serenity.TypeScript;

public class ExportDeclaration(NodeArray<IModifierLike> modifiers, bool isTypeOnly,
    INamedExportBindings exportClause, IExpression moduleSpecifier, ImportAttributes attributes)
        : DeclarationStatement<IDeclarationName>(SyntaxKind.ExportDeclaration, name: null), IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsTypeOnly { get; } = isTypeOnly;
    public INamedExportBindings ExportClause { get; } = exportClause;
    public IExpression ModuleSpecifier { get; } = moduleSpecifier;
    public ImportAttributes ImportAttributes { get; } = attributes;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [ExportClause, Name, ModuleSpecifier, ImportAttributes];
    }
}