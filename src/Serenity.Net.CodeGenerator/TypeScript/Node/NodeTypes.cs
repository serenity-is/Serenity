namespace Serenity.TypeScript;

internal class FunctionBody : Block
{
}


internal class FlowType : object
{
}

internal class QualifiedName : NodeBase, IEntityName
{
    public QualifiedName(IEntityName left, Identifier right)
    {
        Kind = SyntaxKind.QualifiedName;
        Left = left;
        Right = right;
    }

    public IEntityName Left { get; }
    public Identifier Right { get; }
}

internal interface IDeclarationStatement : INamedDeclaration, IStatement
{
    //Node name { get; set; } // Identifier | StringLiteral | NumericLiteral
}

internal class DeclarationStatement(SyntaxKind kind, IDeclarationName name) 
    : NodeBase(kind), IDeclarationStatement, IStatement, INamedDeclaration
{
    public IDeclarationName Name { get; } = name;
}

internal class ComputedPropertyName : NodeBase, IPropertyName
{
    public ComputedPropertyName()
    {
        Kind = SyntaxKind.ComputedPropertyName;
    }

    public IExpression Expression { get; set; }
}

internal class Decorator : NodeBase, IModifierLike
{
    public Decorator()
    {
        Kind = SyntaxKind.Decorator;
    }

    public /*LeftHandSideExpression*/IExpression Expression { get; set; }
}


internal class VariableDeclarationList : NodeBase, IVariableDeclarationList
{
    public VariableDeclarationList()
    {
        Kind = SyntaxKind.VariableDeclarationList;
    }

    public NodeArray<VariableDeclaration> Declarations { get; set; }
}


internal class ObjectBindingPattern : NodeBase, IBindingPattern
{
    public ObjectBindingPattern()
    {
        Kind = SyntaxKind.ObjectBindingPattern;
    }

    public NodeArray<IArrayBindingElement> Elements { get; set; }
}

internal class ArrayBindingPattern : NodeBase, IBindingPattern
{
    public ArrayBindingPattern(NodeArray<IArrayBindingElement> elements)
    {
        Kind = SyntaxKind.ArrayBindingPattern;
        Elements = elements;
    }

    public NodeArray<IArrayBindingElement> Elements { get; }
}

internal class KeywordTypeNode(SyntaxKind kind) : TypeNodeBase(kind), IKeywordTypeNode
{
}

internal interface IInferTypeNode : ITypeNode
{
    TypeParameterDeclaration TypeParameter { get; set; }
}

internal class InferTypeNode() : TypeNodeBase(SyntaxKind.InferType), IInferTypeNode
{
    public TypeParameterDeclaration TypeParameter { get; set; }
}

internal class TypeQueryNode(IEntityName exprName, NodeArray<ITypeNode> typeArguments) : TypeNodeBase(SyntaxKind.TypeQuery)
{
    public IEntityName ExprName { get; } = exprName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
}

internal class TypeLiteralNode : NodeBase, ITypeNode, IHasName
{
    public TypeLiteralNode(NodeArray<ITypeElement> members)
    {
        Kind = SyntaxKind.TypeLiteral;
        Members = members;
    }

    public NodeArray<ITypeElement> Members { get; }
    public IDeclarationName Name { get; }
}


internal interface IUnaryExpression : IExpression
{
}

internal class UnaryExpression(SyntaxKind kind) : Expression(kind), IUnaryExpression
{
}

internal interface IUpdateExpression : IUnaryExpression
{
}

internal interface ILeftHandSideExpression : IUpdateExpression
{
}

internal interface IMemberExpression : ILeftHandSideExpression
{
}

internal interface IPrimaryExpression : IMemberExpression
{
}


internal interface IStatement : INode
{
}

internal class Statement : NodeBase, IStatement
{
}

internal class NotEmittedStatement : Statement
{
    public NotEmittedStatement()
    {
        Kind = SyntaxKind.NotEmittedStatement;
    }
}

internal class EmptyStatement : Statement
{
    public EmptyStatement()
    {
        Kind = SyntaxKind.EmptyStatement;
    }
}

internal class DebuggerStatement : Statement
{
    public DebuggerStatement()
    {
        Kind = SyntaxKind.DebuggerStatement;
    }
}


internal class Block : Statement, IBlockOrExpression
{
    public Block()
    {
        Kind = SyntaxKind.Block;
    }

    public NodeArray<IStatement> Statements { get; set; }
    public bool MultiLine { get; set; }
}

internal class VariableStatement : Statement
{
    public VariableStatement()
    {
        Kind = SyntaxKind.VariableStatement;
    }

    public IVariableDeclarationList DeclarationList { get; set; }
}

internal class ExpressionStatement : Statement
{
    public ExpressionStatement()
    {
        Kind = SyntaxKind.ExpressionStatement;
    }

    public IExpression Expression { get; set; }
}

internal class PrologueDirective : ExpressionStatement
{
}

internal class IfStatement : Statement
{
    public IfStatement()
    {
        Kind = SyntaxKind.IfStatement;
    }

    public IExpression Expression { get; set; }
    public IStatement ThenStatement { get; set; }
    public IStatement ElseStatement { get; set; }
}

internal class IterationStatement : Statement
{
    public IStatement Statement { get; set; }
}

internal class DoStatement : IterationStatement
{
    public DoStatement()
    {
        Kind = SyntaxKind.DoStatement;
    }

    public IExpression Expression { get; set; }
}

internal class WhileStatement : IterationStatement
{
    public WhileStatement()
    {
        Kind = SyntaxKind.WhileStatement;
    }

    public IExpression Expression { get; set; }
}

internal class ForStatement : IterationStatement
{
    public ForStatement()
    {
        Kind = SyntaxKind.ForStatement;
    }

    public /*ForInitializer*/IVariableDeclarationListOrExpression Initializer { get; set; }
    public IExpression Condition { get; set; }
    public IExpression Incrementor { get; set; }
}

internal class ForInStatement : IterationStatement
{
    public ForInStatement()
    {
        Kind = SyntaxKind.ForInStatement;
    }

    public /*ForInitializer*/IVariableDeclarationListOrExpression Initializer { get; set; }
    public IExpression Expression { get; set; }
}

internal class ForOfStatement : IterationStatement
{
    public ForOfStatement()
    {
        Kind = SyntaxKind.ForOfStatement;
    }

    public AwaitKeyword AwaitModifier { get; set; }
    public /*ForInitializer*/IVariableDeclarationListOrExpression Initializer { get; set; }
    public IExpression Expression { get; set; }
}

internal class BreakStatement : Statement, IBreakOrContinueStatement
{
    public BreakStatement()
    {
        Kind = SyntaxKind.BreakStatement;
    }

    public Identifier Label { get; set; }
}

internal class ContinueStatement : Statement, IBreakOrContinueStatement
{
    public ContinueStatement()
    {
        Kind = SyntaxKind.ContinueStatement;
    }

    public Identifier Label { get; set; }
}

internal class ReturnStatement : Statement
{
    public ReturnStatement()
    {
        Kind = SyntaxKind.ReturnStatement;
    }

    public IExpression Expression { get; set; }
}

internal class WithStatement : Statement
{
    public WithStatement()
    {
        Kind = SyntaxKind.WithStatement;
    }

    public IExpression Expression { get; set; }
    public IStatement Statement { get; set; }
}

internal class SwitchStatement : Statement
{
    public SwitchStatement()
    {
        Kind = SyntaxKind.SwitchStatement;
    }

    public IExpression Expression { get; set; }
    public CaseBlock CaseBlock { get; set; }
    public bool PossiblyExhaustive { get; set; }
}

internal class CaseBlock : NodeBase
{
    public CaseBlock()
    {
        Kind = SyntaxKind.CaseBlock;
    }

    public NodeArray<ICaseOrDefaultClause> Clauses { get; set; }
}

internal class CaseClause : NodeBase, ICaseOrDefaultClause
{
    public CaseClause()
    {
        Kind = SyntaxKind.CaseClause;
    }

    public IExpression Expression { get; set; }
    public NodeArray<IStatement> Statements { get; set; }
}

internal class DefaultClause : NodeBase, ICaseOrDefaultClause
{
    public DefaultClause()
    {
        Kind = SyntaxKind.DefaultClause;
    }

    public NodeArray<IStatement> Statements { get; set; }
}

internal class LabeledStatement : Statement
{
    public LabeledStatement()
    {
        Kind = SyntaxKind.LabeledStatement;
    }

    public Identifier Label { get; set; }
    public IStatement Statement { get; set; }
}

internal class ThrowStatement : Statement
{
    public ThrowStatement()
    {
        Kind = SyntaxKind.ThrowStatement;
    }

    public IExpression Expression { get; set; }
}

internal class TryStatement : Statement
{
    public TryStatement()
    {
        Kind = SyntaxKind.TryStatement;
    }

    public Block TryBlock { get; set; }
    public CatchClause CatchClause { get; set; }
    public Block FinallyBlock { get; set; }
}

internal class CatchClause : NodeBase
{
    public CatchClause()
    {
        Kind = SyntaxKind.CatchClause;
    }

    public VariableDeclaration VariableDeclaration { get; set; }
    public Block Block { get; set; }
}

internal interface IClassLikeDeclaration : INamedDeclaration
{
    NodeArray<TypeParameterDeclaration> TypeParameters { get; }
    NodeArray<HeritageClause> HeritageClauses { get; }
    NodeArray<IClassElement> Members { get; }
}


internal interface IClassElement : IDeclaration
{
}

internal class InterfaceDeclaration : DeclarationStatement
{
    public InterfaceDeclaration()
    {
        Kind = SyntaxKind.InterfaceDeclaration;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<HeritageClause> HeritageClauses { get; set; }
    public NodeArray<ITypeElement> Members { get; set; }
}

internal class HeritageClause : NodeBase
{
    public HeritageClause()
    {
        Kind = SyntaxKind.HeritageClause;
    }

    public SyntaxKind Token { get; set; } //  SyntaxKind.ExtendsKeyword | SyntaxKind.ImplementsKeyword
    public NodeArray<ExpressionWithTypeArguments> Types { get; set; }
}

internal class TypeAliasDeclaration : DeclarationStatement
{
    public TypeAliasDeclaration()
    {
        Kind = SyntaxKind.TypeAliasDeclaration;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public ITypeNode Type { get; set; }
}

internal class EnumMember : NamedDeclaration
{
    public EnumMember()
    {
        Kind = SyntaxKind.EnumMember;
    }

    public IExpression Initializer { get; set; }
}

internal class EnumDeclaration : DeclarationStatement
{
    public EnumDeclaration()
    {
        Kind = SyntaxKind.EnumDeclaration;
    }

    public NodeArray<EnumMember> Members { get; set; }
}

internal class ModuleDeclaration : DeclarationStatement
{
    public ModuleDeclaration()
    {
        Kind = SyntaxKind.ModuleDeclaration;
    }

    public /*ModuleDeclaration*/INode Body { get; set; } // ModuleBody | JSDocNamespaceDeclaration
}

internal class NamespaceDeclaration : ModuleDeclaration
{
}

internal class ModuleBlock : Block
{
    public ModuleBlock()
    {
        Kind = SyntaxKind.ModuleBlock;
    }
}


internal class ExternalModuleReference : NodeBase
{
    public ExternalModuleReference()
    {
        Kind = SyntaxKind.ExternalModuleReference;
    }

    public IExpression Expression { get; set; }
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

internal class NamespaceExportDeclaration : DeclarationStatement()
{
    public NamespaceExportDeclaration()
    {
        Kind = SyntaxKind.NamespaceExportDeclaration;
    }
}

internal class ExportDeclaration : DeclarationStatement
{
    public ExportDeclaration()
    {
        Kind = SyntaxKind.ExportDeclaration;
    }

    public NamedExports ExportClause { get; set; }
    public IExpression ModuleSpecifier { get; set; }
}

internal class NamedImports : NodeBase, INamedImportsOrExports, INamedImportBindings
{
    public NamedImports()
    {
        Kind = SyntaxKind.NamedImports;
    }

    public NodeArray<ImportSpecifier> Elements { get; set; }
}

internal class NamedExports : NodeBase, INamedImportsOrExports
{
    public NamedExports()
    {
        Kind = SyntaxKind.NamedExports;
    }

    public NodeArray<ExportSpecifier> Elements { get; set; }
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

internal class ExportAssignment : DeclarationStatement
{
    public ExportAssignment()
    {
        Kind = SyntaxKind.ExportAssignment;
    }

    public bool IsExportEquals { get; set; }
    public IExpression Expression { get; set; }
}

internal class CommentRange : TextRange
{
    public bool HasTrailingNewLine { get; set; }
    public SyntaxKind Kind { get; set; }
}

internal class SynthesizedComment : CommentRange
{
    public string Text { get; set; }
}
