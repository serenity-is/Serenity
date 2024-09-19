namespace Serenity.TypeScript;

public class TypeElement(SyntaxKind kind, IPropertyName name, QuestionToken questionToken)
    : NamedDeclaration<IPropertyName>(kind, name), ITypeElement, IGetRestChildren
{
    public QuestionToken QuestionToken { get; set; } = questionToken;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, QuestionToken];
    }
}
