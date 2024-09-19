namespace Serenity.TypeScript;

public class PrefixUnaryExpression(SyntaxKind @operator, IExpression operand)
    : UpdateExpressionBase(SyntaxKind.PrefixUnaryExpression), IGetRestChildren
{
    public /*PrefixUnaryOperator*/SyntaxKind Operator { get; } = @operator;
    public /*UnaryExpression*/IExpression Operand { get; } = operand;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Operand];
    }
}