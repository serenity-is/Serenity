using System.Runtime.CompilerServices;

namespace Serenity.TypeScript;

internal class Expression(SyntaxKind kind) : NodeBase(kind), IExpression
{
}

internal class OmittedExpression() : Expression(SyntaxKind.OmittedExpression), IArrayBindingElement
{
}

internal class LeftHandSideExpression(SyntaxKind kind) : UpdateExpression(kind), ILeftHandSideExpression
{
}

internal class MemberExpression(SyntaxKind kind) : LeftHandSideExpression(kind), IMemberExpression
{
}

internal class PrimaryExpression(SyntaxKind kind) : MemberExpression(kind), IPrimaryExpression, IJsxTagNameExpression
{
}

internal class PartiallyEmittedExpression(IExpression expression) : LeftHandSideExpression(SyntaxKind.PartiallyEmittedExpression)
{
    public IExpression Expression { get; } = expression;
}

internal class ThisExpression() : PrimaryExpression(SyntaxKind.ThisKeyword), IKeywordTypeNode
{
}

internal class UpdateExpression(SyntaxKind kind) : UnaryExpression(kind), IUpdateExpression
{
}

internal class PrefixUnaryExpression(SyntaxKind @operator, IExpression operand) : UpdateExpression(SyntaxKind.PrefixUnaryExpression)
{
    public /*PrefixUnaryOperator*/SyntaxKind Operator { get; } = @operator;
    public /*UnaryExpression*/IExpression Operand { get; } = operand;
}

internal class PostfixUnaryExpression(IExpression operand, SyntaxKind @operator)
    : UpdateExpression(SyntaxKind.PostfixUnaryExpression)
{
    public /*LeftHandSideExpression*/IExpression Operand { get; } = operand;
    public /*PostfixUnaryOperator*/SyntaxKind Operator { get; } = @operator;
}


internal class SuperExpression() : PrimaryExpression(SyntaxKind.SuperKeyword)
{
}

internal class ImportExpression() : PrimaryExpression(SyntaxKind.ImportKeyword)
{
}

internal class DeleteExpression(IExpression expression) : UnaryExpression(SyntaxKind.DeleteExpression)
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class TypeOfExpression(IExpression expression) : UnaryExpression(SyntaxKind.TypeOfExpression)
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class VoidExpression(IExpression expression) : UnaryExpression(SyntaxKind.VoidExpression)
{

    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class AwaitExpression(IExpression expression) : UnaryExpression(SyntaxKind.AwaitExpression)
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class YieldExpression(AsteriskToken asteriskToken, IExpression expression) : Expression(SyntaxKind.YieldExpression)
{
    public AsteriskToken AsteriskToken { get; } = asteriskToken;
    public IExpression Expression { get; } = expression;
}

internal class BinaryExpression(IExpression left, Token operatorToken, IExpression right) : Expression(SyntaxKind.BinaryExpression), IExpression, IDeclaration
{
    public IExpression Left { get; } = left;
    public /*BinaryOperator*/Token OperatorToken { get; } = operatorToken;
    public IExpression Right { get; } = right;
}

internal class AssignmentExpression(IExpression left, IExpression right) : BinaryExpression(left, new Token(SyntaxKind.EqualsToken), right)
{
}
internal class ObjectDestructuringAssignment(ObjectLiteralExpression left, IExpression right) : AssignmentExpression(left, right);

internal class ArrayDestructuringAssignment(ArrayLiteralExpression left, IExpression right) : AssignmentExpression(left, right);

internal class ConditionalExpression(IExpression condition, QuestionToken questionToken, IExpression whenTrue, ColonToken colonToken, IExpression whenFalse) : Expression(SyntaxKind.ConditionalExpression)
{
    public IExpression Condition { get; } = condition;
    public QuestionToken QuestionToken { get; } = questionToken;
    public IExpression WhenTrue { get; } = whenTrue;
    public ColonToken ColonToken { get; } = colonToken;
    public IExpression WhenFalse { get; } = whenFalse;
}

internal class ParenthesizedExpression(IExpression expression) : PrimaryExpression(SyntaxKind.ParenthesizedExpression)
{
    public IExpression Expression { get; } = expression;
}

internal class ArrayLiteralExpression(NodeArray<IExpression> elements, bool multiLine)
    : PrimaryExpression(SyntaxKind.ArrayLiteralExpression)
{
    public NodeArray<IExpression> Elements { get; } = elements;
    public bool MultiLine { get; } = multiLine;
}

internal class SpreadElement(IExpression expression) : Expression(SyntaxKind.SpreadElement)
{
    public IExpression Expression { get; } = expression;
}

internal class ObjectLiteralExpressionBase<T>(SyntaxKind kind, NodeArray<T> properties) : NodeBase(kind), IPrimaryExpression, IDeclaration
{
    public NodeArray<T> Properties { get; set; } = properties;
}

internal class ObjectLiteralExpression(NodeArray<IObjectLiteralElementLike> properties, bool multiLine)
    : ObjectLiteralExpressionBase<IObjectLiteralElementLike>(SyntaxKind.ObjectLiteralExpression, properties)
{
    public bool MultiLine { get; } = multiLine;
}

internal class PropertyAccessExpression(IExpression expression, Identifier name)
    : NodeBase(SyntaxKind.PropertyAccessExpression), IMemberExpression, IDeclaration, IJsxTagNameExpression
{

    public IExpression Expression { get; } = expression; //LeftHandSideExpression
    public Identifier Name { get; } = name;
}

internal class SuperPropertyAccessExpression(IExpression expression, Identifier name)
    : PropertyAccessExpression(expression, name)
{
}

internal class PropertyAccessEntityNameExpression(IExpression expression, Identifier name)
    : PropertyAccessExpression(expression, name)
{
}

internal class BaseElementAccessExpression(IExpression expression, QuestionToken questionDotToken, IExpression argumentExpression)
    : MemberExpression(SyntaxKind.ElementAccessExpression)
{
    public IExpression Expression { get; } = expression; //LeftHandSideExpression
    public QuestionToken QuestionToken { get; } = questionDotToken;
    public IExpression ArgumentExpression { get; } = argumentExpression;
}

internal class ElementAccessExpression(IExpression expression, IExpression argumentExpression)
    : BaseElementAccessExpression(expression, questionDotToken: null, argumentExpression)
{
}

internal class SuperElementAccessExpression(SuperExpression superExpression, IExpression argumentExpression)
    : ElementAccessExpression(superExpression, argumentExpression)
{
}

internal class CallExpression(IExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : LeftHandSideExpression(SyntaxKind.CallExpression), IMemberExpression, IDeclaration
{
    public /*LeftHandSideExpression*/IExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public NodeArray<IExpression> Arguments { get; } = argumentsArray;
}

internal class SuperCall(SuperExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}

internal class ImportCall(ImportExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}

internal class ExpressionWithTypeArguments(IExpression expression, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.ExpressionWithTypeArguments)
{
    public /*LeftHandSideExpression*/IExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
}

internal class NewExpression(IExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray)
    : PrimaryExpression(SyntaxKind.NewExpression), IDeclaration
{
    public IExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public NodeArray<IExpression> Arguments { get; } = argumentsArray;
}

internal class TaggedTemplateExpression(IExpression tag, NodeArray<ITypeNode> typeArguments, ITemplateLiteral template) : MemberExpression(SyntaxKind.TaggedTemplateExpression)
{
    public IExpression Tag { get; set; } = tag; //LeftHandSideExpression
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public INode Template { get; set; } = template; //TemplateLiteral
}

internal class AsExpression(IExpression expression, ITypeNode type) : Expression(SyntaxKind.AsExpression)
{
    public IExpression Expression { get; } = expression;
    public ITypeNode Type { get; } = type;
}

internal class TypeAssertion(ITypeNode type, IExpression expression) : UnaryExpression(SyntaxKind.TypeAssertionExpression)
{
    public ITypeNode Type { get; } = type;
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class NonNullExpression(IExpression expression) : /*LeftHandSideExpression*/LeftHandSideExpression(SyntaxKind.NonNullExpression)
{
    public IExpression Expression { get; } = expression;
}

internal class MetaProperty(SyntaxKind keywordToken, Identifier name) : PrimaryExpression(SyntaxKind.MetaProperty)
{
    public SyntaxKind KeywordToken { get; } = keywordToken;
    public Identifier Name { get; } = name;
}