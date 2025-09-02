namespace Serenity.TypeScript;

public class PostfixUnaryExpression(IExpression operand, SyntaxKind @operator)
    : UpdateExpressionBase(SyntaxKind.PostfixUnaryExpression), IGetRestChildren
{
    public /*LeftHandSideExpression*/IExpression Operand { get; } = operand;
    public /*PostfixUnaryOperator*/SyntaxKind Operator { get; } = @operator;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Operand];
    }
}