namespace Serenity.Data;

/// <summary>
/// Validates a criteria for allowed field names, operators and SQL injection safety
/// </summary>
/// <remarks>
/// This validator is intended for criteria supplied by clients, such as criteria
/// deserialized from a <c>ListRequest</c>. A criteria instance does not indicate
/// whether it came from JSON or was constructed by application code, so the same
/// restrictions apply in either case. This is not a validator for every criteria
/// expression the framework can generate: aliases (e.g. <c>T0.Name</c>), quoted
/// identifiers, and free-form expressions such as <c>1=1</c> are intentionally
/// rejected, even when they are otherwise valid framework criteria.
/// </remarks>
/// <seealso cref="BaseCriteriaVisitor" />
public class SafeCriteriaValidator : BaseCriteriaVisitor
{
    /// <summary>
    /// Validates the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    public void Validate(BaseCriteria? criteria)
    {
        if (criteria is null || criteria.IsEmpty)
            return;

        Visit(criteria);
    }

    /// <summary>
    /// Visits the criteria, returning a potentially reworked version.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited criteria.</returns>
    /// <exception cref="ValidationError">InvalidCriteriaField</exception>
    protected override BaseCriteria VisitCriteria(Criteria criteria)
    {
        if (string.IsNullOrEmpty(criteria.Expression))
            throw new ValidationError("InvalidCriteriaField",
                "Empty criteria field name is not allowed!");

        // Client criteria is limited to bare field names. Do not broaden this
        // to aliases, quoted identifiers, or SQL expressions: the visitor cannot
        // distinguish a trusted framework-created criteria from client input.
        if (!SqlSyntax.IsValidIdentifier(criteria.Expression))
            throw new ValidationError("InvalidCriteriaField",
                string.Format("{0} is not a valid field name!", criteria.Expression));

        return base.VisitCriteria(criteria);
    }

    /// <summary>
    /// Visits the value criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited value criteria.</returns>
    /// <exception cref="ValidationError">UnsupportedCriteriaType - Criteria values can't contain nested criteria!</exception>
    protected override BaseCriteria VisitValue(ValueCriteria criteria)
    {
        if (ContainsNestedCriteria(criteria.Value))
            throw new ValidationError("UnsupportedCriteriaType",
                "Criteria values can't contain nested criteria!");

        return base.VisitValue(criteria);
    }

    private static bool ContainsNestedCriteria(object? value)
    {
        if (value is BaseCriteria)
            return true;

        // Values may be collections (e.g. an IN list deserialized from JSON),
        // so a nested criteria hidden inside an array must be rejected too.
        if (value is System.Collections.IEnumerable enumerable and not string)
        {
            foreach (var item in enumerable)
            {
                if (ContainsNestedCriteria(item))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Visits the parameter criteria. A parameter criteria is
    /// just a parameter name.
    /// </summary>
    /// <param name="criteria">The parameter criteria.</param>
    /// <returns>The visited parameter criteria.</returns>
    /// <exception cref="ValidationError">UnsupportedCriteriaType - Param type criterias is not supported!</exception>
    protected override BaseCriteria VisitParam(ParamCriteria criteria)
    {
        throw new ValidationError("UnsupportedCriteriaType",
            "Param type criterias is not supported!");
    }

    /// <summary>
    /// Visits an allowed function call criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited function call criteria.</returns>
    /// <exception cref="ValidationError">Only UPPER with one field or string argument is supported.</exception>
    protected override BaseCriteria VisitFunctionCall(FunctionCallCriteria criteria)
    {
        if (criteria is not UpperFunctionCriteria)
            throw new ValidationError("UnsupportedCriteriaType",
                "Only UPPER function criteria is supported!");

        var arguments = criteria.Arguments;
        if (arguments.Length != 1)
            throw new ValidationError("UnsupportedCriteriaType",
                "UPPER function criteria must have exactly one argument!");

        switch (arguments[0])
        {
            case Criteria field:
                Visit(field);
                break;

            case ValueCriteria { Value: string } value:
                Visit(value);
                break;

            default:
                throw new ValidationError("UnsupportedCriteriaType",
                    "UPPER function argument must be a field name or string value!");
        }

        return criteria;
    }
}
