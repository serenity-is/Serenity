namespace Serenity.Data;

/// <summary>
/// Criteria object that identifies a function call
/// </summary>
/// <param name="arguments">The arguments.</param>
public abstract class FunctionCallCriteria(params BaseCriteria[] arguments) : BaseCriteria
{
    private readonly BaseCriteria[] arguments = arguments;

    /// <summary>
    /// Gets the arguments.
    /// </summary>
    public BaseCriteria[] Arguments => arguments;

    /// <inheritdoc/>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        AppendFunctionName(sb, query);
        AppendOpenParenthesis(sb, query);
        AppendArguments(sb, query);
        AppendCloseParenthesis(sb, query);
    }

    /// <summary>
    /// Gets the function name
    /// </summary>
    public abstract string GetFunctionName(ISqlDialect dialect);

    /// <summary>
    /// Appends the function name
    /// </summary>
    /// <param name="sb">String builder</param>
    /// <param name="query">Query with params</param>
    /// <returns></returns>
    protected virtual void AppendFunctionName(StringBuilder sb, IQueryWithParams query)
    {
        sb.Append(GetFunctionName(query.Dialect));
    }

    /// <summary>
    /// Appends the opening parenthesis
    /// </summary>
    /// <param name="sb">String builder</param>
    /// <param name="query">Query with params</param>
    protected virtual void AppendOpenParenthesis(StringBuilder sb, IQueryWithParams query)
    {
        sb.Append('(');
    }

    /// <summary>
    /// Appends the closing parenthesis
    /// </summary>
    /// <param name="sb">String builder</param>
    /// <param name="query">Query with params</param>
    protected virtual void AppendCloseParenthesis(StringBuilder sb, IQueryWithParams query)
    {
        sb.Append(')');
    }

    /// <summary>
    /// Appends the arguments
    /// </summary>
    /// <param name="sb">String builder</param>
    /// <param name="query">Query with params</param>
    protected virtual void AppendArguments(StringBuilder sb, IQueryWithParams query)
    {
        var argIndex = 0;
        foreach (var argument in arguments)
        {
            if (argIndex > 0)
                sb.Append(", ");

            argument.ToString(sb, query);
            argIndex++;
        }
    }
}