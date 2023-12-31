namespace Serenity.TypeScript;

internal class ExternalModuleReference(IExpression expression)
    : Node(SyntaxKind.ExternalModuleReference), IGetRestChildren, IModuleReference
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}