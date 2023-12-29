namespace Serenity.TypeScript;

internal class TextRange : ITextRange
{
    public int? Pos { get; set; }
    public int? End { get; set; }
}

internal class CaseBlock(NodeArray<ICaseOrDefaultClause> clauses)
    : Node(SyntaxKind.CaseBlock), IGetRestChildren
{
    public NodeArray<ICaseOrDefaultClause> Clauses { get; } = clauses;

    public IEnumerable<INode> GetRestChildren()
    {
        if (Clauses != null) foreach (var x in Clauses) yield return x;
    }
}

internal class CaseClause(IExpression expression, NodeArray<IStatement> statements)
    : Node(SyntaxKind.CaseClause), ICaseOrDefaultClause, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public NodeArray<IStatement> Statements { get; } = statements;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Expression;
        if (Statements != null) foreach (var x in Statements) yield return x;
    }
}

internal class Decorator(IExpression expression) : Node(SyntaxKind.Decorator), IModifierLike, IGetRestChildren
{
    public IExpression Expression { get; } = expression; // LeftHandSideExpression

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class ObjectBindingPattern(NodeArray<IArrayBindingElement> elements)
    : Node(SyntaxKind.ObjectBindingPattern), IBindingPattern, IGetRestChildren
{
    public NodeArray<IArrayBindingElement> Elements { get; } = elements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}

internal class ArrayBindingPattern(NodeArray<IArrayBindingElement> elements) 
    : Node(SyntaxKind.ArrayBindingPattern), IBindingPattern, IGetRestChildren
{
    public NodeArray<IArrayBindingElement> Elements { get; } = elements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}

internal interface IInferTypeNode : ITypeNode
{
    TypeParameterDeclaration TypeParameter { get; }
}

internal class TypeQueryNode(IEntityName exprName, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.TypeQuery), IGetRestChildren
{
    public IEntityName ExprName { get; } = exprName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return ExprName;
        if (TypeArguments != null)
            foreach (var x in TypeArguments) yield return x;
    }
}

internal class TypeLiteralNode(NodeArray<ITypeElement> members) 
    : TypeNodeBase(SyntaxKind.TypeLiteral), ITypeNode, IObjectTypeDeclaration, IHasNameProperty, IGetRestChildren
{
    public NodeArray<ITypeElement> Members { get; } = members;
    public IDeclarationName Name { get; }

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}

internal class DefaultClause(NodeArray<IStatement> statements) 
    : Node(SyntaxKind.DefaultClause), ICaseOrDefaultClause, IGetRestChildren
{
    public NodeArray<IStatement> Statements { get; set; } = statements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Statements;
    }
}

internal class CatchClause(VariableDeclaration variableDeclaration, Block block) 
    : Node(SyntaxKind.CatchClause), IGetRestChildren
{
    public VariableDeclaration VariableDeclaration { get; set; } = variableDeclaration;
    public Block Block { get; set; } = block;

    public IEnumerable<INode> GetRestChildren()
    {
        return [VariableDeclaration, Block];
    }
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
