namespace Serenity.TypeScript;

public class MetaProperty(SyntaxKind keywordToken, Identifier name)
    : PrimaryExpressionBase(SyntaxKind.MetaProperty), IHasNameProperty, IGetRestChildren
{
    public SyntaxKind KeywordToken { get; } = keywordToken;
    public Identifier Name { get; } = name;

    IDeclarationName IHasNameProperty.Name => Name;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Name];
    }
}
