
namespace Serenity.TypeScript;

internal class ObjectLiteralElement<TName>(SyntaxKind kind, TName name)
    : NamedDeclaration<TName>(kind, name), IObjectLiteralElement
    where TName : IDeclarationName
{
}

internal class PropertyAssignment(IPropertyName name, IExpression initializer)
    : ObjectLiteralElement<IPropertyName>(SyntaxKind.PropertyAssignment, name), 
    IObjectLiteralElementLike, IVariableLikeDeclaration, IGetRestChildren, IHasModifiers
{
    public IExpression Initializer { get; set; } = initializer;

    public NodeArray<IModifierLike> Modifiers { get; set; } // for error reporting
    public INode QuestionToken { get; set; }  // for error reporting
    public INode ExclamationToken { get; set; }  // for error reporting

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, QuestionToken, Initializer, ExclamationToken];
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
        return [Name, EqualsToken, ObjectAssignmentInitializer, QuestionToken, ExclamationToken];
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
