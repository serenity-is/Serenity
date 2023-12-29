
namespace Serenity.TypeScript;

internal class Statement(SyntaxKind kind)
    : Node(kind), IStatement
{
}

internal class NotEmittedStatement() : Statement(SyntaxKind.NotEmittedStatement)
{
}

internal class EmptyStatement() : Statement(SyntaxKind.EmptyStatement)
{
}

internal class DebuggerStatement() : Statement(SyntaxKind.DebuggerStatement), IFlowContainer
{
}

internal class Block(NodeArray<IStatement> statements, bool? multiLine)
    : Statement(SyntaxKind.Block), IBlockOrExpression, IBlockLike, IGetRestChildren
{
    public NodeArray<IStatement> Statements { get; } = statements;
    public bool? MultiLine { get; } = multiLine;

    public IEnumerable<INode> GetRestChildren()
    {
        return Statements;
    }
}


internal class ModuleBlock(NodeArray<IStatement> statements) 
    : Node(SyntaxKind.ModuleBlock), IBlockLike, IGetRestChildren, INamespaceBody
{
    public NodeArray<IStatement> Statements { get; } = statements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Statements;
    }
}

internal class VariableStatement(NodeArray<IModifierLike> modifiers, IVariableDeclarationList variableDeclarationList)
    : Statement(SyntaxKind.VariableStatement), IFlowContainer, IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers => modifiers;
    public IVariableDeclarationList DeclarationList { get; } = variableDeclarationList;

    public IEnumerable<INode> GetRestChildren()
    {
        return [DeclarationList];
    }
}

internal class ExpressionStatement(IExpression expression)
    : Statement(SyntaxKind.ExpressionStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class IfStatement(IExpression expression, IStatement thenStatement, IStatement elseStatement = null)
    : Statement(SyntaxKind.IfStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public IStatement ThenStatement { get; } = thenStatement;
    public IStatement ElseStatement { get; } = elseStatement;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, ThenStatement, ElseStatement];
    }
}

internal class IterationStatement(SyntaxKind kind, IStatement statement) 
    : Statement(kind), IGetRestChildren
{
    public IStatement Statement { get; } = statement;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return [Statement];
    }
}

internal class DoStatement(IStatement statement, IExpression expression)
    : IterationStatement(SyntaxKind.DoStatement, statement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Statement, Expression];
    }
}

internal class WhileStatement(IExpression expression, IStatement statement)
    : IterationStatement(SyntaxKind.WhileStatement, statement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Statement];
    }
}

internal class ForStatement(IForInitializer initializer,
    IExpression condition, IExpression incrementor, IStatement statement)
    : IterationStatement(SyntaxKind.ForStatement, statement), IFlowContainer, IGetRestChildren
{
    public IForInitializer Initializer { get; } = initializer;
    public IExpression Condition { get; } = condition;
    public IExpression Incrementor { get; } = incrementor;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Initializer, Condition, Incrementor, Statement];
    }
}


internal class ForInStatement(IForInitializer initializer, IExpression expression, IStatement statement)
    : IterationStatement(SyntaxKind.ForInStatement, statement), IFlowContainer, IGetRestChildren
{
    public IForInitializer Initializer { get; } = initializer;
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Initializer, Expression, Statement];
    }
}

internal class ForOfStatement(AwaitKeyword awaitKeyword, IForInitializer initializer,
    IExpression expression, IStatement statement)
    : IterationStatement(SyntaxKind.ForOfStatement, statement), IFlowContainer, IGetRestChildren
{
    public AwaitKeyword AwaitKeyword { get; } = awaitKeyword;
    public IForInitializer Initializer { get; } = initializer;
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [AwaitKeyword, Initializer, Expression, Statement];
    }
}

internal class BreakStatement(Identifier label = null)
    : Statement(SyntaxKind.BreakStatement), IBreakOrContinueStatement, IFlowContainer, IGetRestChildren
{
    public Identifier Label { get; } = label;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Label];
    }
}

internal class ContinueStatement(Identifier label = null)
    : Statement(SyntaxKind.ContinueStatement), IBreakOrContinueStatement, IFlowContainer, IGetRestChildren
{

    public Identifier Label { get; } = label;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Label];
    }
}

internal class ReturnStatement(IExpression expression)
    : Statement(SyntaxKind.ReturnStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class WithStatement(IExpression expression, IStatement statement)
    : Statement(SyntaxKind.WithStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public IStatement Statement { get; } = statement;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Statement];
    }
}

internal class SwitchStatement(IExpression expression, CaseBlock caseBlock, bool possiblyExhaustive = false)
    : Statement(SyntaxKind.SwitchStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public CaseBlock CaseBlock { get; } = caseBlock;
    public bool PossiblyExhaustive { get; } = possiblyExhaustive;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, CaseBlock];
    }
}

internal class LabeledStatement(Identifier label, IStatement statement)
    : Statement(SyntaxKind.LabeledStatement), IFlowContainer, IGetRestChildren
{
    public Identifier Label { get; } = label;
    public IStatement Statement { get; } = statement;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Label, Statement];
    }
}

internal class ThrowStatement(IExpression expression)
    : Statement(SyntaxKind.ThrowStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}

internal class TryStatement(Block tryBlock, CatchClause catchClause, Block finallyBlock)
    : Statement(SyntaxKind.TryStatement), IFlowContainer, IGetRestChildren
{
    public Block TryBlock { get; } = tryBlock;
    public CatchClause CatchClause { get; } = catchClause;
    public Block FinallyBlock { get; } = finallyBlock;

    public IEnumerable<INode> GetRestChildren()
    {
        return [TryBlock, CatchClause, FinallyBlock];
    }
}
