namespace Serenity.TypeScript;

internal class ObjectLiteralElement(SyntaxKind kind, IDeclarationName name)
    : NamedDeclaration(kind, name), IObjectLiteralElement
{
}

internal class PropertyAssignment(IDeclarationName name, IExpression initializer)
    : ObjectLiteralElement(SyntaxKind.PropertyAssignment, name), IObjectLiteralElementLike, IVariableLikeDeclaration
{
    public IExpression Initializer { get; set; } = initializer;
}

internal class ShorthandPropertyAssignment(Identifier name, IExpression objectAssignmentInitializer)
    : ObjectLiteralElement(SyntaxKind.ShorthandPropertyAssignment, name), IObjectLiteralElementLike
{
    public EqualsToken EqualsToken { get; set; }
    public IExpression ObjectAssignmentInitializer { get; } = objectAssignmentInitializer;
    public QuestionToken QuestionToken { get; set; }
    public QuestionToken ExclamationToken { get; set; }
}

internal class SpreadAssignment(IExpression expression) 
    : ObjectLiteralElement(SyntaxKind.SpreadAssignment, null), IObjectLiteralElementLike
{
    public IExpression Expression { get; set; } = expression;
}
