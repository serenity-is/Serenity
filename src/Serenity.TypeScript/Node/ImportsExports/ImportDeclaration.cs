namespace Serenity.TypeScript;

public class ImportDeclaration(NodeArray<IModifierLike> modifiers, ImportClause importClause,
    IExpression moduleSpecifier, ImportAttributes attributes)
    : Statement(SyntaxKind.ImportDeclaration), IGetRestChildren, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; set; } = modifiers;
    public ImportClause ImportClause { get; } = importClause;
    public IExpression ModuleSpecifier { get; } = moduleSpecifier;
    public ImportAttributes Attributes { get; } = attributes;

    public IEnumerable<INode> GetRestChildren()
    {
        return [ImportClause, ModuleSpecifier, Attributes];
    }
}
