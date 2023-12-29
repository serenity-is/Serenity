
namespace Serenity.TypeScript;

internal class ObjectLiteralElement<TName>(SyntaxKind kind, TName name)
    : NamedDeclaration<TName>(kind, name), IObjectLiteralElement
    where TName : IDeclarationName
{
}

internal class PropertyAssignment(IPropertyName name, IExpression initializer)
    : ObjectLiteralElement<IPropertyName>(SyntaxKind.PropertyAssignment, name), 
    IObjectLiteralElementLike, IVariableLikeDeclaration, IGetRestChildren
{
    public IExpression Initializer { get; set; } = initializer;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Initializer];
    }
}

internal class ShorthandPropertyAssignment(Identifier name, IExpression objectAssignmentInitializer)
    : ObjectLiteralElement<Identifier>(SyntaxKind.ShorthandPropertyAssignment, name), 
    IObjectLiteralElementLike, IGetRestChildren
{
    public EqualsToken EqualsToken { get; set; }
    public IExpression ObjectAssignmentInitializer { get; } = objectAssignmentInitializer;
    public QuestionToken QuestionToken { get; set; }
    public ExclamationToken ExclamationToken { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, ObjectAssignmentInitializer, QuestionToken, ExclamationToken];
    }
}

internal class SpreadAssignment(IExpression expression) 
    : ObjectLiteralElement<IPropertyName>(SyntaxKind.SpreadAssignment, null), IObjectLiteralElementLike, IGetRestChildren
{
    public IExpression Expression { get; set; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Expression];
    }
}
