namespace Serenity.TypeScript;

public class ShorthandPropertyAssignment(Identifier name, IExpression objectAssignmentInitializer)
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