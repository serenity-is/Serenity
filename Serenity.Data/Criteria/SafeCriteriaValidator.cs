namespace Serenity.Data
{
    using Serenity.Services;
    using System;

    public class SafeCriteriaValidator : BaseCriteriaVisitor
    {
        public void Validate(BaseCriteria criteria)
        {
            Visit(criteria);
        }

        protected override BaseCriteria VisitCriteria(Criteria criteria)
        {
            if (criteria.Expression.IsEmptyOrNull())
                return base.VisitCriteria(criteria);

            if (!SqlSyntax.IsValidIdentifier(criteria.Expression))
                throw new ValidationError("InvalidCriteriaField",
                    String.Format("{0} is not a valid field name!", criteria.Expression));

            return base.VisitCriteria(criteria);
        }

        protected override BaseCriteria VisitParam(ParamCriteria criteria)
        {
            throw new ValidationError("UnsupportedCriteriaType",
                "Param type criterias is not supported!");
        }
    }
}