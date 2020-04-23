namespace Serenity.Data
{
    using System;

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
        /// <returns></returns>
        /// <exception cref="Exception">Criteria type is unkown.</exception>
        protected virtual BaseCriteria Visit(BaseCriteria criteria)
        {
            if (Object.ReferenceEquals(null, criteria))
                return null;

            if (criteria is Criteria)
                return VisitCriteria((Criteria)criteria);

            if (criteria is BinaryCriteria)
                return VisitBinary((BinaryCriteria)criteria);

            if (criteria is UnaryCriteria)
                return VisitUnary((UnaryCriteria)criteria);

            if (criteria is ValueCriteria)
                return VisitValue((ValueCriteria)criteria);

            if (criteria is ParamCriteria)
                return VisitParam((ParamCriteria)criteria);

            throw new Exception(String.Format("Unhandled criteria type: '{0}'", criteria.GetType().Name));
        }

        /// <summary>
        /// Visits the criteria returning potentially reworked version.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected virtual BaseCriteria VisitCriteria(Criteria criteria)
        {
            return criteria;
        }

        /// <summary>
        /// Visits the binary criteria. Binary criteria is one with
        /// two operands and an operator between.
        /// </summary>
        /// <param name="criteria">The binary criteria.</param>
        /// <returns></returns>
        protected virtual BaseCriteria VisitBinary(BinaryCriteria criteria)
        {
            var left = this.Visit(criteria.LeftOperand);
            var right = this.Visit(criteria.RightOperand);

            if (!Object.ReferenceEquals(left, criteria.LeftOperand) ||
                !Object.ReferenceEquals(right, criteria.RightOperand))
            {
                return new BinaryCriteria(left, criteria.Operator, right);
            }

            return criteria;
        }

        /// <summary>
        /// Visits the unary criteria. Unary criteria is one with
        /// one operand and one operator.
        /// </summary>
        /// <param name="criteria">The unary criteria.</param>
        /// <returns></returns>
        protected virtual BaseCriteria VisitUnary(UnaryCriteria criteria)
        {
            var operand = this.Visit(criteria.Operand);
            if (!Object.ReferenceEquals(operand, criteria.Operand))
            {
                return new UnaryCriteria(criteria.Operator, operand);
            }

            return criteria;
        }

        /// <summary>
        /// Visits the parameter criteria. Parameter criteria is
        /// just a parameter name.
        /// </summary>
        /// <param name="criteria">The parameter criteria.</param>
        /// <returns></returns>
        protected virtual BaseCriteria VisitParam(ParamCriteria criteria)
        {
            return criteria;
        }

        /// <summary>
        /// Visits the value criteria. Value criteria is just a constant
        /// value.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected virtual BaseCriteria VisitValue(ValueCriteria criteria)
        {
            return criteria;
        }
    }
}