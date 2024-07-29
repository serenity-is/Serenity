namespace Serenity.TypeScript;

public class ExportSpecifier(bool isTypeOnly, IModuleExportName propertyName, IModuleExportName name)
    : NamedDeclaration<IModuleExportName>(SyntaxKind.ExportSpecifier, name), IImportOrExportSpecifier, IGetRestChildren
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public IModuleExportName PropertyName { get; } = propertyName;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [PropertyName, Name];
    }
}