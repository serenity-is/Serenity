namespace Serenity.TypeScript;

public class ExportSpecifier(bool isTypeOnly, Identifier propertyName, Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.ExportSpecifier, name), IImportOrExportSpecifier, IGetRestChildren
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public Identifier PropertyName { get; } = propertyName;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [PropertyName, Name];
    }
}