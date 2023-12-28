namespace Serenity.TypeScript;

internal class ImportEqualsDeclaration(NodeArray<IModifierLike> modifiers, bool isTypeOnly, IDeclarationName name,
    IModuleReference moduleReference) : DeclarationStatement(SyntaxKind.ImportEqualsDeclaration, name)
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsTypeOnly { get; } = isTypeOnly;
    public IModuleReference ModuleReference { get; set; } = moduleReference;
}

internal class ImportClause(bool isTypeOnly, Identifier name, INamedImportBindings namedBindings)
    : NamedDeclaration(SyntaxKind.ImportClause, name)
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public INamedImportBindings NamedBindings { get; } = namedBindings;
}

internal class NamespaceImport(Identifier name)
    : NamedDeclaration(SyntaxKind.NamespaceImport, name), INamedImportBindings
{
}

internal class NamespaceExport(Identifier name)
    : NamedDeclaration(SyntaxKind.NamespaceExport, name), INamedExportBindings
{
}

internal class ImportAttribute(IDeclarationName name, IExpression value)
    : NodeBase(SyntaxKind.ImportAttribute)
{
    public IDeclarationName Name { get; } = name;
    public IExpression Value { get; } = value;
}

internal class ImportAttributes(NodeArray<ImportAttribute> elements, bool multiLine, SyntaxKind token = SyntaxKind.WithKeyword)
    : NodeBase(SyntaxKind.ImportAttributes)
{
    public NodeArray<ImportAttribute> Elements { get; } = elements;
    public bool MultiLine { get; } = multiLine;
    public SyntaxKind Token { get; } = token;
}

internal class ExportDeclaration(NodeArray<IModifierLike> modifiers, bool isTypeOnly,
    INamedExportBindings exportClause, IExpression moduleSpecifier, ImportAttributes attributes)
        : DeclarationStatement(SyntaxKind.ExportDeclaration, name: null)
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsTypeOnly { get; } = isTypeOnly;
    public INamedExportBindings ExportClause { get; } = exportClause;
    public IExpression ModuleSpecifier { get; } = moduleSpecifier;
    public ImportAttributes ImportAttributes { get; } = attributes;
}

internal class NamedImports(NodeArray<ImportSpecifier> elements)
    : NodeBase(SyntaxKind.NamedImports), INamedImportsOrExports, INamedImportBindings
{
    public NodeArray<ImportSpecifier> Elements { get; } = elements;
}

internal class NamedExports(NodeArray<ExportSpecifier> elements)
    : NodeBase(SyntaxKind.NamedExports), INamedImportsOrExports
{
    public NodeArray<ExportSpecifier> Elements { get; } = elements;
}

internal class ImportSpecifier(bool isTypeOnly, Identifier propertyName, Identifier name)
    : NamedDeclaration(SyntaxKind.ImportSpecifier, name), IImportOrExportSpecifier
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public Identifier PropertyName { get; } = propertyName;
}

internal class ExportSpecifier(bool isTypeOnly, Identifier propertyName, Identifier name)
    : NamedDeclaration(SyntaxKind.ExportSpecifier, name), IImportOrExportSpecifier
{
    public bool IsTypeOnly { get; } = isTypeOnly;
    public Identifier PropertyName { get; } = propertyName;
}

internal class ImportDeclaration : Statement
{
    public ImportDeclaration()
    {
        Kind = SyntaxKind.ImportDeclaration;
    }

    public ImportClause ImportClause { get; set; }
    public IExpression ModuleSpecifier { get; set; }
}

internal class ExportAssignment(NodeArray<IModifierLike> modifiers, bool isExportEquals,  IExpression expression) 
    : DeclarationStatement(SyntaxKind.ExportAssignment, name: null)
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsExportEquals { get; } = isExportEquals;
    public IExpression Expression { get; } = expression;
}

internal class NodeWithTypeArguments(SyntaxKind kind, NodeArray<ITypeNode> typeArguments) : TypeNodeBase(kind)
{
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
}

internal class ImportTypeNode(ITypeNode argument, ImportAttributes attributes, IEntityName qualifier, 
    NodeArray<ITypeNode> typeArguments, bool isTypeOf = false)
    : NodeWithTypeArguments(SyntaxKind.ImportType, typeArguments)
{
    public ITypeNode Argument { get; } = argument;
    public ImportAttributes Attributes { get; } = attributes;
    public IEntityName Qualifier { get; } = qualifier;
    public bool IsTypeOf { get; } = isTypeOf;
}