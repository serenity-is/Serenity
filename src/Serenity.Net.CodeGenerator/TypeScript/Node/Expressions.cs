using System.Runtime.CompilerServices;

namespace Serenity.TypeScript;

internal class ExpressionBase(SyntaxKind kind) : NodeBase(kind), IExpression
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

internal class PrimaryExpressionBase(SyntaxKind kind) : MemberExpressionBase(kind), IPrimaryExpression, IJsxTagNameExpression
{
}

internal class PartiallyEmittedExpression(IExpression expression) 
    : LeftHandSideExpressionBase(SyntaxKind.PartiallyEmittedExpression)
{
    public IExpression Expression { get; } = expression;
}

internal class ThisExpression() 
    : PrimaryExpressionBase(SyntaxKind.ThisKeyword), IKeywordTypeNode
{
}

internal class UpdateExpressionBase(SyntaxKind kind) 
    : UnaryExpressionBase(kind), IUpdateExpression
{
}

internal class PrefixUnaryExpression(SyntaxKind @operator, IExpression operand) 
    : UpdateExpressionBase(SyntaxKind.PrefixUnaryExpression)
{
    public /*PrefixUnaryOperator*/SyntaxKind Operator { get; } = @operator;
    public /*UnaryExpression*/IExpression Operand { get; } = operand;
}

internal class PostfixUnaryExpression(IExpression operand, SyntaxKind @operator)
    : UpdateExpressionBase(SyntaxKind.PostfixUnaryExpression)
{
    public /*LeftHandSideExpression*/IExpression Operand { get; } = operand;
    public /*PostfixUnaryOperator*/SyntaxKind Operator { get; } = @operator;
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
    : UnaryExpressionBase(SyntaxKind.DeleteExpression)
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class TypeOfExpression(IExpression expression) : UnaryExpressionBase(SyntaxKind.TypeOfExpression)
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class VoidExpression(IExpression expression) : UnaryExpressionBase(SyntaxKind.VoidExpression)
{

    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class AwaitExpression(IExpression expression) : UnaryExpressionBase(SyntaxKind.AwaitExpression)
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class YieldExpression(AsteriskToken asteriskToken, IExpression expression) : ExpressionBase(SyntaxKind.YieldExpression)
{
    public AsteriskToken AsteriskToken { get; } = asteriskToken;
    public IExpression Expression { get; } = expression;
}

internal class BinaryExpression(IExpression left, Token operatorToken, IExpression right) : ExpressionBase(SyntaxKind.BinaryExpression), IExpression, IDeclaration
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

internal class ConditionalExpression(IExpression condition, QuestionToken questionToken, IExpression whenTrue, ColonToken colonToken, IExpression whenFalse) : ExpressionBase(SyntaxKind.ConditionalExpression)
{
    public IExpression Condition { get; } = condition;
    public QuestionToken QuestionToken { get; } = questionToken;
    public IExpression WhenTrue { get; } = whenTrue;
    public ColonToken ColonToken { get; } = colonToken;
    public IExpression WhenFalse { get; } = whenFalse;
}

internal class ParenthesizedExpression(IExpression expression) : PrimaryExpressionBase(SyntaxKind.ParenthesizedExpression)
{
    public IExpression Expression { get; } = expression;
}

internal class ArrayLiteralExpression(NodeArray<IExpression> elements, bool multiLine)
    : PrimaryExpressionBase(SyntaxKind.ArrayLiteralExpression)
{
    public NodeArray<IExpression> Elements { get; } = elements;
    public bool MultiLine { get; } = multiLine;
}

internal class SpreadElement(IExpression expression) : ExpressionBase(SyntaxKind.SpreadElement)
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
    : MemberExpressionBase(SyntaxKind.ElementAccessExpression)
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
    NodeArray<IExpression> argumentsArray) : LeftHandSideExpressionBase(SyntaxKind.CallExpression), IMemberExpression, IDeclaration
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
    : TypeNodeBase(SyntaxKind.ExpressionWithTypeArguments), IMemberExpression
{
    public /*LeftHandSideExpression*/IExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
}

internal class NewExpression(IExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray)
    : PrimaryExpressionBase(SyntaxKind.NewExpression), IDeclaration
{
    public IExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public NodeArray<IExpression> Arguments { get; } = argumentsArray;
}

internal class TaggedTemplateExpression(IExpression tag, NodeArray<ITypeNode> typeArguments, ITemplateLiteral template) : MemberExpressionBase(SyntaxKind.TaggedTemplateExpression)
{
    public IExpression Tag { get; set; } = tag; //LeftHandSideExpression
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public INode Template { get; set; } = template; //TemplateLiteral
}

internal class AsExpression(IExpression expression, ITypeNode type) : ExpressionBase(SyntaxKind.AsExpression)
{
    public IExpression Expression { get; } = expression;
    public ITypeNode Type { get; } = type;
}

internal class TypeAssertion(ITypeNode type, IExpression expression) : UnaryExpressionBase(SyntaxKind.TypeAssertionExpression)
{
    public ITypeNode Type { get; } = type;
    public /*UnaryExpression*/IExpression Expression { get; } = expression;
}

internal class NonNullExpression(IExpression expression) : /*LeftHandSideExpression*/LeftHandSideExpressionBase(SyntaxKind.NonNullExpression)
{
    public IExpression Expression { get; } = expression;
}

internal class MetaProperty(SyntaxKind keywordToken, Identifier name) : PrimaryExpressionBase(SyntaxKind.MetaProperty)
{
    public SyntaxKind KeywordToken { get; } = keywordToken;
    public Identifier Name { get; } = name;
}

internal class SatisfiesExpression(IExpression expression, ITypeNode type)
    : ExpressionBase(SyntaxKind.SatisfiesExpression)
{
    public IExpression Expression { get; } = expression;
    public ITypeNode Type { get; } = type;
}