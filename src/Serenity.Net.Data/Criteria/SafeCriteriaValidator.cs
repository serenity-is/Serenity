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
    public void Validate(BaseCriteria criteria)
    {
        Visit(criteria);
    }

    /// <summary>
    /// Visits the criteria returning potentially reworked version.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns></returns>
    /// <exception cref="ValidationError">InvalidCriteriaField</exception>
    protected override BaseCriteria VisitCriteria(Criteria criteria)
    {
        if (criteria.Expression.IsEmptyOrNull())
            return base.VisitCriteria(criteria);

        if (!SqlSyntax.IsValidIdentifier(criteria.Expression))
            throw new ValidationError("InvalidCriteriaField",
                string.Format("{0} is not a valid field name!", criteria.Expression));

        return base.VisitCriteria(criteria);
    }

    /// <summary>
    /// Visits the parameter criteria. Parameter criteria is
    /// just a parameter name.
    /// </summary>
    /// <param name="criteria">The parameter criteria.</param>
    /// <returns></returns>
    /// <exception cref="ValidationError">UnsupportedCriteriaType - Param type criterias is not supported!</exception>
    protected override BaseCriteria VisitParam(ParamCriteria criteria)
    {
        throw new ValidationError("UnsupportedCriteriaType",
            "Param type criterias is not supported!");
    }
}