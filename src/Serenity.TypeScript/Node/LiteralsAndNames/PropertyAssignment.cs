namespace Serenity.TypeScript;

public class PropertyAssignment(IPropertyName name, IExpression initializer)
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
