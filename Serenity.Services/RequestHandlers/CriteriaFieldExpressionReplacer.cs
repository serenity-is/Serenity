namespace Serenity.Data
{
    using Serenity.Services;
    using System;

    public class CriteriaFieldExpressionReplacer : SafeCriteriaValidator
    {
        public CriteriaFieldExpressionReplacer(Row row)
        {
            this.Row = row;
        }

        protected Row Row { get; private set; }

        public BaseCriteria Process(BaseCriteria criteria)
        {
            return Visit(criteria);
        }

        protected virtual bool CanFilterField(Field field)
        {
            if (field.Flags.HasFlag(FieldFlags.DenyFiltering))
                return false;

            if (field.MinSelectLevel == SelectLevel.Never)
                return false;

            return true;
        }

        protected virtual Field FindField(string expression)
        {
            return Row.FindFieldByPropertyName(expression) ?? Row.FindField(expression);
        }

        protected override BaseCriteria VisitCriteria(Criteria criteria)
        {
            var result = base.VisitCriteria(criteria);
            
            criteria = result as Criteria;

            if (!Object.ReferenceEquals(null, criteria))
            {
                var field = FindField(criteria.Expression);
                if (field == null || !CanFilterField(field))
                {
                    throw new ValidationError("InvalidCriteriaField", criteria.Expression,
                        String.Format("'{0}' criteria field is not found!", criteria.Expression));
                }

                return new Criteria(field);
            }

            return result;
        }
    }
}