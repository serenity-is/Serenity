namespace Serenity.Data;

/// <summary>
/// Criteria object that identifies a function call
/// </summary>
/// <param name="arguments">The arguments.</param>
public abstract class FunctionCallCriteria(params BaseCriteria[] arguments) : BaseCriteria
{
    private BaseCriteria[] arguments = CheckArguments(arguments);

    /// <summary>
    /// Gets a copy of the arguments. Mutating the returned array does not
    /// affect this instance.
    /// </summary>
    public BaseCriteria[] Arguments => (BaseCriteria[])arguments.Clone();

    private static BaseCriteria[] CheckArguments(BaseCriteria[]? arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        foreach (var argument in arguments)
            ArgumentNullException.ThrowIfNull(argument, nameof(arguments));

        // Defensive copy: the caller may keep and mutate the passed array.
        return (BaseCriteria[])arguments.Clone();
    }

    /// <summary>
    /// Creates a copy of this function criteria with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments for the copy.</param>
    /// <returns>A copy of this function criteria.</returns>
    /// <exception cref="ArgumentNullException">arguments is null.</exception>
    protected internal virtual FunctionCallCriteria CloneWithArguments(BaseCriteria[] arguments)
    {
        var clone = (FunctionCallCriteria)MemberwiseClone();
        clone.arguments = CheckArguments(arguments);
        return clone;
    }

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
    /// Appends the function name.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The query with params.</param>
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