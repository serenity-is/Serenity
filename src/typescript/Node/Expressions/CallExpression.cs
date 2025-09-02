namespace Serenity.TypeScript;

public class CallExpression(IExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray)
    : LeftHandSideExpressionBase(SyntaxKind.CallExpression), IMemberExpression, IDeclaration, IGetRestChildren
{
    public CallExpression(IExpression expression, QuestionDotToken questionDotToken, NodeArray<ITypeNode> typeArguments,
        NodeArray<IExpression> argumentsArray)
        : this(expression, typeArguments, argumentsArray)
    {
        QuestionDotToken = questionDotToken;
    }

    public /*LeftHandSideExpression*/IExpression Expression { get; } = expression;
    public QuestionDotToken QuestionDotToken { get; }
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public NodeArray<IExpression> Arguments { get; } = argumentsArray;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Expression;
        yield return QuestionDotToken;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
        if (Arguments != null) foreach (var x in Arguments) yield return x;
    }
}