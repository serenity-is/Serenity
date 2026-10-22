namespace Serenity.TypeScript;

public class ImportClause(SyntaxKind? phaseModifier, Identifier? name, INamedImportBindings? namedBindings)
    : NamedDeclaration<Identifier>(SyntaxKind.ImportClause, name), IGetRestChildren
{
    public SyntaxKind? PhaseModifier { get; } = phaseModifier;
    public bool IsTypeOnly => PhaseModifier == SyntaxKind.TypeKeyword;
    public INamedImportBindings? NamedBindings { get; } = namedBindings;

    public override IEnumerable<INode?> GetRestChildren()
    {
        return [Name, NamedBindings];
    }
}
