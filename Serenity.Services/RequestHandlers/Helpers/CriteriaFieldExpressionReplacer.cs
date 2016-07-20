namespace Serenity.Data
{
    using Serenity.Services;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

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
                if (ReferenceEquals(null, field) || !CanFilterField(field))
                {
                    throw new ValidationError("InvalidCriteriaField", criteria.Expression,
                        String.Format("'{0}' criteria field is not found!", criteria.Expression));
                }

                return new Criteria(field);
            }

            return result;
        }

        private bool ShouldConvertValues(BinaryCriteria criteria, out Field field, out object value)
        {
            field = null;
            value = null;

            if (ReferenceEquals(null, criteria) ||
                criteria.Operator < CriteriaOperator.EQ ||
                criteria.Operator > CriteriaOperator.NotIn)
                return false;

            var left = criteria.LeftOperand as Criteria;
            if (ReferenceEquals(null, left))
                return false;

            field = FindField(left.Expression);
            if (ReferenceEquals(null, field))
                return false;

            if (field is StringField)
                return false;

            var right = criteria.RightOperand as ValueCriteria;
            if (ReferenceEquals(null, right))
                return false;

            value = right.Value;
            return value != null;
        }

        protected override BaseCriteria VisitBinary(BinaryCriteria criteria)
        {
            Field field;
            object value;
            if (ShouldConvertValues(criteria, out field, out value))
            try
            {
                var str = value as string;
                if (str == null && value is IEnumerable)
                {
                    var values = new List<object>();
                    foreach (var v in value as IEnumerable)
                        values.Add(field.ConvertValue(v, CultureInfo.InvariantCulture));

                    return new BinaryCriteria(new Criteria(field), criteria.Operator, new ValueCriteria(values));
                }

                if (str == null || str.Length != 0)
                {
                    value = field.ConvertValue(value, CultureInfo.InvariantCulture);
                    return new BinaryCriteria(new Criteria(field), criteria.Operator, new ValueCriteria(value));
                }
            }
            catch
            {
                // swallow exceptions for backward compability
            }

            return base.VisitBinary(criteria);
        }
    }
}