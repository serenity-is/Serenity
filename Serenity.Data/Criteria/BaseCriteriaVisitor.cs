namespace Serenity.Data
{
    using System;

    public abstract class BaseCriteriaVisitor
    {
        protected BaseCriteriaVisitor()
        {
        }

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

        protected virtual BaseCriteria VisitCriteria(Criteria criteria)
        {
            return criteria;
        }

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
        
        protected virtual BaseCriteria VisitUnary(UnaryCriteria criteria)
        {
            var operand = this.Visit(criteria.Operand);
            if (!Object.ReferenceEquals(operand, criteria.Operand))
            {
                return new UnaryCriteria(criteria.Operator, operand);
            }

            return criteria;
        }

        protected virtual BaseCriteria VisitParam(ParamCriteria criteria)
        {
            return criteria;
        }

        protected virtual BaseCriteria VisitValue(ValueCriteria criteria)
        {
            return criteria;
        }
    }
}