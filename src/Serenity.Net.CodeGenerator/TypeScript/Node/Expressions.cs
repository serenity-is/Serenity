
namespace Serenity.TypeScript;

internal class ExpressionBase(SyntaxKind kind) : Node(kind), IExpression
{
}

internal class OmittedExpression() : ExpressionBase(SyntaxKind.OmittedExpression), IArrayBindingElement
{
}

internal class LeftHandSideExpressionBase(SyntaxKind kind) : UpdateExpressionBase(kind), ILeftHandSideExpression
{
}

internal class MemberExpressionBase(SyntaxKind kind) : LeftHandSideExpressionBase(kind), IMemberExpression
{
}

internal class PrimaryExpressionBase(SyntaxKind kind) 
    : MemberExpressionBase(kind), IPrimaryExpression, IJsxTagNameExpression
{
}

internal class PartiallyEmittedExpression(IExpression expression)
    : LeftHandSideExpressionBase(SyntaxKind.PartiallyEmittedExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class ThisExpression()
    : PrimaryExpressionBase(SyntaxKind.ThisKeyword), IKeywordTypeNode
{
}

internal class UnaryExpressionBase(SyntaxKind kind) : ExpressionBase(kind), IUnaryExpression
{
}

internal class UpdateExpressionBase(SyntaxKind kind)
    : UnaryExpressionBase(kind), IUpdateExpression
{
}

internal class PrefixUnaryExpression(SyntaxKind @operator, IExpression operand)
    : UpdateExpressionBase(SyntaxKind.PrefixUnaryExpression), IGetRestChildren
{
    public /*PrefixUnaryOperator*/SyntaxKind Operator { get; } = @operator;
    public /*UnaryExpression*/IExpression Operand { get; } = operand;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Operand];
    }
}

internal class PostfixUnaryExpression(IExpression operand, SyntaxKind @operator)
    : UpdateExpressionBase(SyntaxKind.PostfixUnaryExpression), IGetRestChildren
{
    public /*LeftHandSideExpression*/IExpression Operand { get; } = operand;
    public /*PostfixUnaryOperator*/SyntaxKind Operator { get; } = @operator;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Operand];
    }
}

internal class SuperExpression()
    : PrimaryExpressionBase(SyntaxKind.SuperKeyword), IMemberExpression
{
}

internal class ImportExpression()
    : PrimaryExpressionBase(SyntaxKind.ImportKeyword)
{
}

internal class DeleteExpression(IExpression expression)
    : UnaryExpressionBase(SyntaxKind.DeleteExpression), IGetRestChildren
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class TypeOfExpression(IExpression expression) : UnaryExpressionBase(SyntaxKind.TypeOfExpression), IGetRestChildren
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class VoidExpression(IExpression expression) : UnaryExpressionBase(SyntaxKind.VoidExpression), IGetRestChildren
{

    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class AwaitExpression(IExpression expression) 
    : UnaryExpressionBase(SyntaxKind.AwaitExpression), IGetRestChildren
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class YieldExpression(AsteriskToken asteriskToken, IExpression expression)
    : ExpressionBase(SyntaxKind.YieldExpression), IGetRestChildren
{
    public AsteriskToken AsteriskToken { get; } = asteriskToken;
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [AsteriskToken, Expression];
    }
}

internal class BinaryExpression(IExpression left, Token operatorToken, IExpression right)
    : ExpressionBase(SyntaxKind.BinaryExpression), IExpression, IDeclaration, IGetRestChildren
{
    public IExpression Left { get; } = left;
    public /*BinaryOperator*/Token OperatorToken { get; } = operatorToken;
    public IExpression Right { get; } = right;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Left, OperatorToken, Right];
    }
}

internal class AssignmentExpression(IExpression left, IExpression right)
    : BinaryExpression(left, new Token(SyntaxKind.EqualsToken), right)
{
}

internal class ObjectDestructuringAssignment(ObjectLiteralExpression left, IExpression right)
    : AssignmentExpression(left, right);

internal class ArrayDestructuringAssignment(ArrayLiteralExpression left, IExpression right)
    : AssignmentExpression(left, right);

internal class ConditionalExpression(IExpression condition, QuestionToken questionToken,
    IExpression whenTrue, ColonToken colonToken, IExpression whenFalse)
    : ExpressionBase(SyntaxKind.ConditionalExpression), IGetRestChildren
{
    public IExpression Condition { get; } = condition;
    public QuestionToken QuestionToken { get; } = questionToken;
    public IExpression WhenTrue { get; } = whenTrue;
    public ColonToken ColonToken { get; } = colonToken;
    public IExpression WhenFalse { get; } = whenFalse;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Condition, QuestionToken, WhenTrue, ColonToken, WhenFalse];
    }
}

internal class ParenthesizedExpression(IExpression expression)
    : PrimaryExpressionBase(SyntaxKind.ParenthesizedExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class ArrayLiteralExpression(NodeArray<IExpression> elements, bool multiLine)
    : PrimaryExpressionBase(SyntaxKind.ArrayLiteralExpression), IGetRestChildren
{
    public NodeArray<IExpression> Elements { get; } = elements;
    public bool MultiLine { get; } = multiLine;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}

internal class SpreadElement(IExpression expression) 
    : ExpressionBase(SyntaxKind.SpreadElement), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class ObjectLiteralExpressionBase<T>(SyntaxKind kind, NodeArray<T> properties) 
    : Node(kind), IPrimaryExpression, IDeclaration, IGetRestChildren where T: INode
{
    public NodeArray<T> Properties { get; set; } = properties;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        if (Properties != null) foreach (var x in Properties) yield return x;
    }
}

internal class ObjectLiteralExpression(NodeArray<IObjectLiteralElementLike> properties, bool multiLine)
    : ObjectLiteralExpressionBase<IObjectLiteralElementLike>(SyntaxKind.ObjectLiteralExpression, properties)
{
    public bool MultiLine { get; } = multiLine;
}

internal class PropertyAccessExpression(IExpression expression, Identifier name) 
    : NamedDeclaration<Identifier>(SyntaxKind.PropertyAccessExpression, name), 
        IMemberExpression, IJsxTagNameExpression, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public QuestionDotToken QuestionDotToken { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Expression, QuestionDotToken, Name];
    }
}

internal class PropertyAccessChain : PropertyAccessExpression
{
    public PropertyAccessChain(IExpression expression, QuestionDotToken questionDotToken, Identifier name)
        : base(expression, name)
    {
        Flags |= NodeFlags.OptionalChain;
        QuestionDotToken = questionDotToken;
    }
}

internal class ElementAccessExpression(IExpression expression, IExpression argumentExpression)
    : MemberExpressionBase(SyntaxKind.ElementAccessExpression), IGetRestChildren
{
    public ElementAccessExpression(IExpression expression, QuestionDotToken questionDotToken, IExpression argumentExpression)
        : this(expression, argumentExpression)
    {
        QuestionDotToken = questionDotToken;
    }

    public IExpression Expression { get; } = expression; //LeftHandSideExpression
    public QuestionDotToken QuestionDotToken { get; }
    public IExpression ArgumentExpression { get; } = argumentExpression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, QuestionDotToken, ArgumentExpression];
    }
}

internal class ElementAccessChain : ElementAccessExpression
{
    public ElementAccessChain(IExpression superExpression, QuestionDotToken questionDotToken, IExpression argumentExpression)
        : base(superExpression, questionDotToken, argumentExpression)
    {
        Flags |= NodeFlags.OptionalChain;
    }
}

internal class CallExpression(IExpression expression, NodeArray<ITypeNode> typeArguments,
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

internal class CallChain(IExpression expression, QuestionDotToken questionDotToken, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray)
    : CallExpression(expression, questionDotToken, typeArguments, argumentsArray)
{
}

internal class SuperCall(SuperExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}

internal class ImportCall(ImportExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}

internal class ExpressionWithTypeArguments(ILeftHandSideExpression expression, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.ExpressionWithTypeArguments), IMemberExpression, IGetRestChildren
{
    public ILeftHandSideExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Expression;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
    }
}

internal class NewExpression(IExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray)
    : PrimaryExpressionBase(SyntaxKind.NewExpression), IDeclaration, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public NodeArray<IExpression> Arguments { get; } = argumentsArray;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Expression;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
        if (Arguments != null) foreach (var x in Arguments) yield return x;
    }
}

internal class TaggedTemplateExpression(IExpression tag, NodeArray<ITypeNode> typeArguments, INode template)
    : MemberExpressionBase(SyntaxKind.TaggedTemplateExpression), IGetRestChildren
{
    public IExpression Tag { get; set; } = tag; //LeftHandSideExpression
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public INode Template { get; set; } = template; //TemplateLiteral
    public QuestionDotToken QuestionDotToken { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Tag;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
        yield return Template;
        yield return QuestionDotToken;
    }
}

internal class AsExpression(IExpression expression, ITypeNode type) 
    : ExpressionBase(SyntaxKind.AsExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Type];
    }
}

internal class TypeAssertion(ITypeNode type, IExpression expression) 
    : UnaryExpressionBase(SyntaxKind.TypeAssertionExpression), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type, Expression];
    }
}

internal class NonNullExpression(IExpression expression) : 
    LeftHandSideExpressionBase(SyntaxKind.NonNullExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class MetaProperty(SyntaxKind keywordToken, Identifier name)
    : PrimaryExpressionBase(SyntaxKind.MetaProperty), IHasNameProperty, IGetRestChildren
{
    public SyntaxKind KeywordToken { get; } = keywordToken;
    public Identifier Name { get; } = name;

    IDeclarationName IHasNameProperty.Name => Name;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Name];
    }
}

internal class SatisfiesExpression(IExpression expression, ITypeNode type)
    : ExpressionBase(SyntaxKind.SatisfiesExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Type];
    }
}