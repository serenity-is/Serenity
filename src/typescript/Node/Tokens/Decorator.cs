namespace Serenity.TypeScript;

public class Decorator(IExpression expression)
    : Node(SyntaxKind.Decorator), IModifierLike, IGetRestChildren
{
    public IExpression Expression { get; } = expression; // LeftHandSideExpression

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}