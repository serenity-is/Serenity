
namespace Serenity.TypeScript;

public class SwitchStatement(IExpression expression, CaseBlock caseBlock, bool possiblyExhaustive = false)
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
