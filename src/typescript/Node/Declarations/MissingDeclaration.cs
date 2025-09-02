namespace Serenity.TypeScript;

public class MissingDeclaration() : NamedDeclaration<Identifier>(SyntaxKind.MissingDeclaration, null),
    IDeclarationStatement, IClassElement, IObjectLiteralElement, ITypeElement, IHasModifiers, IGetRestChildren, IPrimaryExpression
{
    public NodeArray<IModifierLike> Modifiers { get; set; }
    public QuestionToken QuestionToken { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, QuestionToken];
    }
}