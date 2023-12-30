namespace Serenity.TypeScript;

internal class ImportEqualsDeclaration(NodeArray<IModifierLike> modifiers, bool isTypeOnly, Identifier name,
    IModuleReference moduleReference) : DeclarationStatement<Identifier>(SyntaxKind.ImportEqualsDeclaration, name), IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsTypeOnly { get; } = isTypeOnly;
    public IModuleReference ModuleReference { get; set; } = moduleReference;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, ModuleReference];
    }
}

internal class ImportClause(bool isTypeOnly, Identifier name, INamedImportBindings namedBindings)
    : NamedDeclaration<Identifier>(SyntaxKind.ImportClause, name), IGetRestChildren
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public INamedImportBindings NamedBindings { get; } = namedBindings;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, NamedBindings];
    }
}

internal class NamespaceImport(Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.NamespaceImport, name), INamedImportBindings
{
}

internal class NamespaceExport(Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.NamespaceExport, name), INamedExportBindings
{
}

internal class ImportAttribute(IDeclarationName name, IExpression value)
    : Node(SyntaxKind.ImportAttribute), IGetRestChildren, IHasNameProperty
{
    public IDeclarationName Name { get; } = name;
    public IExpression Value { get; } = value;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Name, Value];
    }
}

internal class ImportAttributes(NodeArray<ImportAttribute> elements, bool multiLine,
    SyntaxKind token = SyntaxKind.WithKeyword) : Node(SyntaxKind.ImportAttributes), IGetRestChildren
{
    public NodeArray<ImportAttribute> Elements { get; } = elements;
    public bool MultiLine { get; } = multiLine;
    public SyntaxKind Token { get; } = token;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}

internal class ExportDeclaration(NodeArray<IModifierLike> modifiers, bool isTypeOnly,
    INamedExportBindings exportClause, IExpression moduleSpecifier, ImportAttributes attributes)
        : DeclarationStatement<IDeclarationName>(SyntaxKind.ExportDeclaration, name: null), IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsTypeOnly { get; } = isTypeOnly;
    public INamedExportBindings ExportClause { get; } = exportClause;
    public IExpression ModuleSpecifier { get; } = moduleSpecifier;
    public ImportAttributes ImportAttributes { get; } = attributes;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [ExportClause, Name, ModuleSpecifier, ImportAttributes];
    }
}

internal class NamedImports(NodeArray<ImportSpecifier> elements)
    : Node(SyntaxKind.NamedImports), INamedImportsOrExports, INamedImportBindings, IGetRestChildren
{
    public NodeArray<ImportSpecifier> Elements { get; } = elements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}

internal class NamedExports(NodeArray<ExportSpecifier> elements)
    : Node(SyntaxKind.NamedExports), INamedImportsOrExports, INamedExportBindings, IGetRestChildren
{
    public NodeArray<ExportSpecifier> Elements { get; } = elements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}

internal class ImportSpecifier(bool isTypeOnly, Identifier propertyName, Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.ImportSpecifier, name), IImportOrExportSpecifier, IGetRestChildren
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public Identifier PropertyName { get; } = propertyName;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [PropertyName, Name];
    }
}

internal class ExportSpecifier(bool isTypeOnly, Identifier propertyName, Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.ExportSpecifier, name), IImportOrExportSpecifier, IGetRestChildren
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public Identifier PropertyName { get; } = propertyName;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [PropertyName, Name];
    }
}

internal class ImportDeclaration(NodeArray<IModifierLike> modifiers, ImportClause importClause, 
    IExpression moduleSpecifier, ImportAttributes attributes)
    : Statement(SyntaxKind.ImportDeclaration), IGetRestChildren, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; set; } = modifiers;
    public ImportClause ImportClause { get; } = importClause;
    public IExpression ModuleSpecifier { get; } = moduleSpecifier;
    public ImportAttributes Attributes { get; } = attributes;

    public IEnumerable<INode> GetRestChildren()
    {
        return [ImportClause, ModuleSpecifier, Attributes];
    }
}

internal class ExportAssignment(NodeArray<IModifierLike> modifiers, bool isExportEquals, IExpression expression)
    : DeclarationStatement<IDeclarationName>(SyntaxKind.ExportAssignment, name: null), IGetRestChildren, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsExportEquals { get; } = isExportEquals;
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Name];
    }
}

internal class NodeWithTypeArguments(SyntaxKind kind, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(kind), IGetRestChildren
{
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return TypeArguments ?? [];
    }
}

internal class ImportTypeNode(ITypeNode argument, ImportAttributes attributes, IEntityName qualifier,
    NodeArray<ITypeNode> typeArguments, bool isTypeOf = false)
    : NodeWithTypeArguments(SyntaxKind.ImportType, typeArguments), IGetRestChildren
{
    public ITypeNode Argument { get; } = argument;
    public ImportAttributes Attributes { get; } = attributes;
    public IEntityName Qualifier { get; } = qualifier;
    public bool IsTypeOf { get; } = isTypeOf;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Argument;
        yield return Attributes;
        yield return Qualifier;
        foreach (var x in base.GetRestChildren())
            yield return x;
    }
}

internal class ExternalModuleReference(IExpression expression) 
    : Node(SyntaxKind.ExternalModuleReference), IGetRestChildren, IModuleReference
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}