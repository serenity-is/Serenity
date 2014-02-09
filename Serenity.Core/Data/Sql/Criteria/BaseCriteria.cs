namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class BaseCriteria
    {
        private static NoParamsChecker noParamsChecker = new NoParamsChecker();
        private static IgnoreParams ignoreParams = new IgnoreParams();

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public BaseCriteria IsNull()
        {
            return new UnaryCriteria(CriteriaOperator.IsNull, this);
        }

        public BaseCriteria IsNotNull()
        {
            return new UnaryCriteria(CriteriaOperator.IsNotNull, this);
        }

        public BaseCriteria Like(string mask)
        {
            return new BinaryCriteria(this, CriteriaOperator.Like, new ValueCriteria(mask));
        }

        public BaseCriteria NotLike(string mask)
        {
            return new BinaryCriteria(this, CriteriaOperator.NotLike, new ValueCriteria(mask));
        }

        public BaseCriteria StartsWith(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            return Like(mask + "%");
        }

        public BaseCriteria EndsWith(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            return Like("%" + mask);
        }

        public BaseCriteria Contains(string mask)
        {
            return Like("%" + mask + "%");
        }

        public BaseCriteria NotContains(string mask)
        {
            return NotLike("%" + mask + "%");
        }

        public BaseCriteria In<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            return new BinaryCriteria(this, CriteriaOperator.In, new ValueCriteria(values));
        }

        public BaseCriteria InStatement(BaseCriteria statement)
        {
            if (Object.ReferenceEquals(null, statement) || statement.IsEmpty)
                throw new ArgumentNullException("statement");

            return new BinaryCriteria(this, CriteriaOperator.In, statement); 
        }

        public BaseCriteria NotIn<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            return new BinaryCriteria(this, CriteriaOperator.NotIn, new ValueCriteria(values));
        }

        public BaseCriteria NotInStatement(BaseCriteria statement)
        {
            if (Object.ReferenceEquals(null, statement) || statement.IsEmpty)
                throw new ArgumentNullException("statement");

            return new BinaryCriteria(this, CriteriaOperator.NotIn, statement);
        }

        public static BaseCriteria operator !(BaseCriteria criteria)
        {
            return new UnaryCriteria(CriteriaOperator.Not, criteria);
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, criteria2);
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, Parameter param)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ParamCriteria(param.Name));
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, int value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, Int64 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, string value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, Double value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, Decimal value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, DateTime value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
        }

        public static BaseCriteria operator ==(BaseCriteria criteria1, Guid value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, criteria2);
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, Parameter param)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ParamCriteria(param.Name));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, int value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, Int64 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, string value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, Double value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, Decimal value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, DateTime value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
        }

        public static BaseCriteria operator !=(BaseCriteria criteria1, Guid value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, criteria2);
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, Parameter param)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ParamCriteria(param.Name));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, Int32 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, Int64 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, string value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, Double value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, Decimal value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, DateTime value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
        }

        public static BaseCriteria operator >(BaseCriteria criteria1, Guid value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, criteria2);
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, Parameter param)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ParamCriteria(param.Name));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, Int32 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, Int64 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, string value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, Double value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, Decimal value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, DateTime value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
        }

        public static BaseCriteria operator >=(BaseCriteria criteria1, Guid value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, criteria2);
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, Parameter param)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ParamCriteria(param.Name));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, Int32 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, Int64 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, string value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, Double value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, Decimal value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, DateTime value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
        }

        public static BaseCriteria operator <(BaseCriteria criteria1, Guid value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, criteria2);
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, Parameter param)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ParamCriteria(param.Name));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, Int32 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, Int64 value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, string value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, Double value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, Decimal value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, DateTime value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        public static BaseCriteria operator <=(BaseCriteria criteria1, Guid value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        private static BaseCriteria JoinIf(BaseCriteria criteria1, BaseCriteria criteria2, CriteriaOperator op)
        {
            if (Object.ReferenceEquals(null, criteria1))
                throw new ArgumentNullException("criteria1");

            if (Object.ReferenceEquals(null, criteria2))
                throw new ArgumentNullException("criteria2");

            if (criteria1.IsEmpty)
                return criteria2;
            else if (criteria2.IsEmpty)
                return criteria1;
            else
                return new BinaryCriteria(criteria1, op, criteria2);
        }
        
        public static BaseCriteria operator &(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return JoinIf(criteria1, criteria2, CriteriaOperator.AND);
        }

        public static BaseCriteria operator |(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return JoinIf(criteria1, criteria2, CriteriaOperator.OR);
        }

        public static BaseCriteria operator ^(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return JoinIf(criteria1, criteria2, CriteriaOperator.XOR);
        }

        public static BaseCriteria operator ~(BaseCriteria criteria)
        {
            if (!criteria.IsEmpty)
                return new UnaryCriteria(CriteriaOperator.Paren, criteria);
            else
                return criteria;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public string ToStringCheckNoParams()
        {
            return ToString(noParamsChecker);
        }

        public string ToString(IDbParameterized query)
        {
            var sb = new StringBuilder(256);
            ToString(sb, query);
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(ignoreParams);
        }

        public virtual void ToString(StringBuilder sb, IDbParameterized query)
        {
            throw new NotImplementedException();
        }

        private class NoParamsChecker : IDbParameterized
        {
            public void AddParam(string name, object value)
            {
                throw new InvalidOperationException("Criteria should not have parameters!");
            }

            public void SetParam(string name, object value)
            {
                throw new InvalidOperationException("Criteria should not have parameters!");
            }

            public Parameter AutoParam()
            {
                throw new InvalidOperationException("Criteria should not have parameters!");
            }

            public IDictionary<string, object> Params
            {
                get { return null; }
            }

            public SqlDialect Dialect
            {
                get { return SqlSettings.CurrentDialect; }
            }
        }

        private class IgnoreParams : IDbParameterized
        {
            private static int next;

            public void AddParam(string name, object value)
            {
            }

            public void SetParam(string name, object value)
            {
            }

            public Parameter AutoParam()
            {
                return new Parameter((next++).IndexParam());
            }

            public IDictionary<string, object> Params
            {
                get { return null; }
            }

            public SqlDialect Dialect
            {
                get { return SqlSettings.CurrentDialect; }
            }
        }
    }
}