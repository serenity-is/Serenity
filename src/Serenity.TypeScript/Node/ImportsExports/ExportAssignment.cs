namespace Serenity.TypeScript;

public class ExportAssignment(NodeArray<IModifierLike> modifiers, bool isExportEquals, IExpression expression)
    : DeclarationStatement<IDeclarationName>(SyntaxKind.ExportAssignment, name: null), IGetRestChildren, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsExportEquals { get; } = isExportEquals;
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Name];
    }
}
