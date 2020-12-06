using System;

namespace Serenity.Data
{
    public abstract partial class Field
    {
        public BaseCriteria IsNull()
        {
            return Criteria.IsNull();
        }

        public BaseCriteria IsNotNull()
        {
            return Criteria.IsNotNull();
        }

        public BaseCriteria Like(string mask)
        {
            return Criteria.Like(mask);
        }

        public BaseCriteria NotLike(string mask)
        {
            return Criteria.NotLike(mask);
        }

        public BaseCriteria StartsWith(string mask)
        {
            return Criteria.StartsWith(mask);
        }

        public BaseCriteria EndsWith(string mask)
        {
            return Criteria.EndsWith(mask);
        }

        public BaseCriteria Contains(string mask)
        {
            return Criteria.Contains(mask);
        }

        public BaseCriteria NotContains(string mask)
        {
            return criteria.NotContains(mask);
        }

        public BaseCriteria In<T>(params T[] values)
        {
            return Criteria.In(values);
        }

        public BaseCriteria NotIn<T>(params T[] values)
        {
            return Criteria.NotIn(values);
        }

        public static BaseCriteria operator ==(Field field, BaseCriteria criteria)
        {
            return field.Criteria == criteria;
        }

        public static BaseCriteria operator ==(Field field, DateTime value)
        {
            return field.Criteria == value;
        }

        public static BaseCriteria operator ==(Field field, decimal value)
        {
            return field.Criteria == value;
        }

        public static BaseCriteria operator ==(Field field, double value)
        {
            return field.Criteria == value;
        }

        public static BaseCriteria operator ==(Field field, Field field2)
        {
            return field.Criteria == field2.Criteria;
        }

        public static BaseCriteria operator ==(Field field, Guid value)
        {
            return field.Criteria == value;
        }

        public static BaseCriteria operator ==(Field field, int value)
        {
            return field.Criteria == value;
        }

        public static BaseCriteria operator ==(Field field, long value)
        {
            return field.Criteria == value;
        }

        public static BaseCriteria operator ==(Field field, Parameter param)
        {
            return field.Criteria == param;
        }

        public static BaseCriteria operator ==(Field field, string value)
        {
            return field.Criteria == value;
        }

        public static BaseCriteria operator !=(Field field, BaseCriteria criteria)
        {
            return field.Criteria != criteria;
        }

        public static BaseCriteria operator !=(Field field, DateTime value)
        {
            return field.Criteria != value;
        }

        public static BaseCriteria operator !=(Field field, decimal value)
        {
            return field.Criteria != value;
        }

        public static BaseCriteria operator !=(Field field, double value)
        {
            return field.Criteria != value;
        }

        public static BaseCriteria operator !=(Field field, Field field2)
        {
            return field.Criteria != field2.Criteria;
        }

        public static BaseCriteria operator !=(Field field, Guid value)
        {
            return field.Criteria != value;
        }

        public static BaseCriteria operator !=(Field field, int value)
        {
            return field.Criteria != value;
        }

        public static BaseCriteria operator !=(Field field, long value)
        {
            return field.Criteria != value;
        }

        public static BaseCriteria operator !=(Field field, Parameter param)
        {
            return field.Criteria != param;
        }

        public static BaseCriteria operator !=(Field field, string value)
        {
            return field.Criteria != value;
        }

        public static BaseCriteria operator <(Field field, BaseCriteria criteria)
        {
            return field.Criteria < criteria;
        }

        public static BaseCriteria operator <(Field field, DateTime value)
        {
            return field.Criteria < value;
        }

        public static BaseCriteria operator <(Field field, decimal value)
        {
            return field.Criteria < value;
        }

        public static BaseCriteria operator <(Field field, double value)
        {
            return field.Criteria < value;
        }

        public static BaseCriteria operator <(Field field, Field field2)
        {
            return field.Criteria < field2.Criteria;
        }

        public static BaseCriteria operator <(Field field, Guid value)
        {
            return field.Criteria < value;
        }

        public static BaseCriteria operator <(Field field, int value)
        {
            return field.Criteria < value;
        }

        public static BaseCriteria operator <(Field field, long value)
        {
            return field.Criteria < value;
        }

        public static BaseCriteria operator <(Field field, Parameter param)
        {
            return field.Criteria < param;
        }

        public static BaseCriteria operator <(Field field, string value)
        {
            return field.Criteria < value;
        }

        public static BaseCriteria operator <=(Field field, BaseCriteria criteria)
        {
            return field.Criteria <= criteria;
        }

        public static BaseCriteria operator <=(Field field, DateTime value)
        {
            return field.Criteria <= value;
        }

        public static BaseCriteria operator <=(Field field, decimal value)
        {
            return field.Criteria <= value;
        }

        public static BaseCriteria operator <=(Field field, double value)
        {
            return field.Criteria <= value;
        }

        public static BaseCriteria operator <=(Field field, Field field2)
        {
            return field.Criteria <= field2.Criteria;
        }

        public static BaseCriteria operator <=(Field field, Guid value)
        {
            return field.Criteria <= value;
        }

        public static BaseCriteria operator <=(Field field, int value)
        {
            return field.Criteria <= value;
        }

        public static BaseCriteria operator <=(Field field, long value)
        {
            return field.Criteria <= value;
        }

        public static BaseCriteria operator <=(Field field, Parameter param)
        {
            return field.Criteria <= param;
        }

        public static BaseCriteria operator <=(Field field, string value)
        {
            return field.Criteria <= value;
        }

        public static BaseCriteria operator >(Field field, BaseCriteria criteria)
        {
            return field.Criteria > criteria;
        }

        public static BaseCriteria operator >(Field field, DateTime value)
        {
            return field.Criteria > value;
        }

        public static BaseCriteria operator >(Field field, decimal value)
        {
            return field.Criteria > value;
        }

        public static BaseCriteria operator >(Field field, double value)
        {
            return field.Criteria > value;
        }

        public static BaseCriteria operator >(Field field, Field field2)
        {
            return field.Criteria > field2.Criteria;
        }

        public static BaseCriteria operator >(Field field, Guid value)
        {
            return field.Criteria > value;
        }

        public static BaseCriteria operator >(Field field, int value)
        {
            return field.Criteria > value;
        }

        public static BaseCriteria operator >(Field field, long value)
        {
            return field.Criteria > value;
        }

        public static BaseCriteria operator >(Field field, Parameter param)
        {
            return field.Criteria > param;
        }

        public static BaseCriteria operator >(Field field, string value)
        {
            return field.Criteria > value;
        }

        public static BaseCriteria operator >=(Field field, BaseCriteria criteria)
        {
            return field.Criteria >= criteria;
        }

        public static BaseCriteria operator >=(Field field, DateTime value)
        {
            return field.Criteria >= value;
        }

        public static BaseCriteria operator >=(Field field, decimal value)
        {
            return field.Criteria >= value;
        }

        public static BaseCriteria operator >=(Field field, double value)
        {
            return field.Criteria >= value;
        }

        public static BaseCriteria operator >=(Field field, Field field2)
        {
            return field.Criteria >= field2.Criteria;
        }

        public static BaseCriteria operator >=(Field field, Guid value)
        {
            return field.Criteria >= value;
        }

        public static BaseCriteria operator >=(Field field, int value)
        {
            return field.Criteria >= value;
        }

        public static BaseCriteria operator >=(Field field, long value)
        {
            return field.Criteria >= value;
        }

        public static BaseCriteria operator >=(Field field, Parameter param)
        {
            return field.Criteria >= param;
        }

        public static BaseCriteria operator >=(Field field, string value)
        {
            return field.Criteria >= value;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}