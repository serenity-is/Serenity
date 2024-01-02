namespace Serenity.TypeScript;

public class ImportClause(bool isTypeOnly, Identifier name, INamedImportBindings namedBindings)
    : NamedDeclaration<Identifier>(SyntaxKind.ImportClause, name), IGetRestChildren
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public INamedImportBindings NamedBindings { get; } = namedBindings;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, NamedBindings];
    }
}
