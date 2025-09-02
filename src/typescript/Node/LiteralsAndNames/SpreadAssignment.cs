namespace Serenity.TypeScript;

public class SpreadAssignment(IExpression expression)
    : ObjectLiteralElement<IPropertyName>(SyntaxKind.SpreadAssignment, null), IObjectLiteralElementLike, IGetRestChildren
{
    public IExpression Expression { get; set; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Expression];
    }
}
