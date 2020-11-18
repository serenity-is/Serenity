namespace Serenity.Data
{
    using Serenity.Abstractions;
    using Serenity.Services;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    public class CriteriaFieldExpressionReplacer : SafeCriteriaValidator
    {
        private readonly IPermissionService permissions;

        public CriteriaFieldExpressionReplacer(IRow row, IPermissionService permissions)
        {
            Row = row;
            this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        protected IRow Row { get; private set; }

        public BaseCriteria Process(BaseCriteria criteria)
        {
            return Visit(criteria);
        }

        protected virtual bool CanFilterField(Field field)
        {
            if (field.Flags.HasFlag(FieldFlags.DenyFiltering) ||
                field.Flags.HasFlag(FieldFlags.NotMapped))
                return false;

            if (field.MinSelectLevel == SelectLevel.Never)
                return false;

            if (field.ReadPermission != null &&
                !permissions.HasPermission(field.ReadPermission))
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

            if (criteria is object)
            {
                var field = FindField(criteria.Expression);
                if (field is null)
                {
                    throw new ValidationError("InvalidCriteriaField", criteria.Expression,
                        string.Format("'{0}' criteria field is not found!", criteria.Expression));
                }

                if (!CanFilterField(field))
                {
                    throw new ValidationError("CantFilterField", criteria.Expression,
                        string.Format("Can't filter on field '{0}'!", criteria.Expression));
                }

                return new Criteria(field);
            }

            return result;
        }

        private bool ShouldConvertValues(BinaryCriteria criteria, out Field field, out object value)
        {
            field = null;
            value = null;

            if (criteria is null ||
                criteria.Operator < CriteriaOperator.EQ ||
                criteria.Operator > CriteriaOperator.NotIn)
                return false;

            if (!(criteria.LeftOperand is Criteria left))
                return false;

            field = FindField(left.Expression);
            if (field is null)
                return false;

            if (field is StringField)
                return false;

            if (!(criteria.RightOperand is ValueCriteria right))
                return false;

            value = right.Value;
            return value != null;
        }

        protected override BaseCriteria VisitBinary(BinaryCriteria criteria)
        {
            if (ShouldConvertValues(criteria, out Field field, out object value))
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