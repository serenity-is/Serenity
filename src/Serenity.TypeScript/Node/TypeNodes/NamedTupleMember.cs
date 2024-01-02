namespace Serenity.TypeScript;

public class NamedTupleMember(DotDotDotToken dotDotDotToken, Identifier name, QuestionToken questionToken, ITypeNode type)
    : TypeNodeBase(SyntaxKind.NamedTupleMember), IDeclaration, IGetRestChildren
{
    public DotDotDotToken DotDotDotToken { get; set; } = dotDotDotToken;
    public Identifier Name { get; set; } = name;
    public QuestionToken QuestionToken { get; set; } = questionToken;
    public ITypeNode Type { get; set; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [DotDotDotToken, Name, QuestionToken, Type];
    }
}