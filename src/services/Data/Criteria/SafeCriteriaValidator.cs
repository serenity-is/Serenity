namespace Serenity.Data;

/// <summary>
/// Validates a criteria for allowed field names, operators and SQL injection safety
/// </summary>
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
    /// Visits the function call criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited function call criteria.</returns>
    /// <exception cref="ValidationError">UnsupportedCriteriaType - Function call type criterias is not supported!</exception>
    protected override BaseCriteria VisitFunctionCall(FunctionCallCriteria criteria)
    {
        throw new ValidationError("UnsupportedCriteriaType",
            "Function call type criterias is not supported!");
    }
}