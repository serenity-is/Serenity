namespace Serenity.TypeScript;

public class ImportSpecifier(bool isTypeOnly, Identifier propertyName, Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.ImportSpecifier, name), IImportOrExportSpecifier, IGetRestChildren
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public Identifier PropertyName { get; } = propertyName;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [PropertyName, Name];
    }
}