namespace Serenity.Data;

/// <summary>
/// Criteria object that identifies an UPPER function call
/// </summary>
/// <param name="expression">Expression</param>
public class UpperFunctionCriteria(BaseCriteria expression) : FunctionCallCriteria(expression)
{
    /// <inheritdoc/>
    public override string GetFunctionName(ISqlDialect dialect)
    {
        return "UPPER";
    }
}