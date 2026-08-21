namespace Serenity.Data;

/// <summary>
/// A visitor implementation for BaseCriteria object trees.
/// </summary>
public abstract class BaseCriteriaVisitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCriteriaVisitor"/> class.
    /// </summary>
    protected BaseCriteriaVisitor()
    {
    }

    /// <summary>
    /// Visits the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited criteria, which may be a reworked version.</returns>
    /// <exception cref="Exception">Criteria type is unknown.</exception>
    protected virtual BaseCriteria Visit(BaseCriteria criteria)
    {
        if (criteria is null)
            return null;

        if (criteria is Criteria crit)
            return VisitCriteria(crit);

        if (criteria is BinaryCriteria binary)
            return VisitBinary(binary);

        if (criteria is UnaryCriteria unary)
            return VisitUnary(unary);

        if (criteria is ValueCriteria value)
            return VisitValue(value);

        if (criteria is ParamCriteria prm)
            return VisitParam(prm);

        if (criteria is FunctionCallCriteria func)
            return VisitFunctionCall(func);

        throw new InvalidProgramException(string.Format("Unhandled criteria type: '{0}'", criteria.GetType().Name));
    }

    /// <summary>
    /// Visits the criteria, returning a potentially reworked version.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited criteria.</returns>
    protected virtual BaseCriteria VisitCriteria(Criteria criteria)
    {
        return criteria;
    }

    /// <summary>
    /// Visits the binary criteria. A binary criteria is one with
    /// two operands and an operator between them.
    /// </summary>
    /// <param name="criteria">The binary criteria.</param>
    /// <returns>The visited binary criteria.</returns>
    protected virtual BaseCriteria VisitBinary(BinaryCriteria criteria)
    {
        var left = Visit(criteria.LeftOperand);
        var right = Visit(criteria.RightOperand);

        if (!Object.ReferenceEquals(left, criteria.LeftOperand) ||
            !Object.ReferenceEquals(right, criteria.RightOperand))
        {
            return new BinaryCriteria(left, criteria.Operator, right);
        }

        return criteria;
    }

    /// <summary>
    /// Visits the unary criteria. A unary criteria is one with
    /// one operand and one operator.
    /// </summary>
    /// <param name="criteria">The unary criteria.</param>
    /// <returns>The visited unary criteria.</returns>
    protected virtual BaseCriteria VisitUnary(UnaryCriteria criteria)
    {
        var operand = Visit(criteria.Operand);
        if (!Object.ReferenceEquals(operand, criteria.Operand))
        {
            return new UnaryCriteria(criteria.Operator, operand);
        }

        return criteria;
    }

    /// <summary>
    /// Visits the function call criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited function call criteria.</returns>
    protected virtual BaseCriteria VisitFunctionCall(FunctionCallCriteria criteria)
    {
        return criteria;
    }

    /// <summary>
    /// Visits the parameter criteria. A parameter criteria is
    /// just a parameter name.
    /// </summary>
    /// <param name="criteria">The parameter criteria.</param>
    /// <returns>The visited parameter criteria.</returns>
    protected virtual BaseCriteria VisitParam(ParamCriteria criteria)
    {
        return criteria;
    }

    /// <summary>
    /// Visits the value criteria. A value criteria is just a constant
    /// value.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>The visited value criteria.</returns>
    protected virtual BaseCriteria VisitValue(ValueCriteria criteria)
    {
        return criteria;
    }
}