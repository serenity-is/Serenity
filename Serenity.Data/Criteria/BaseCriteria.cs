namespace Serenity.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Base criteria object type
    /// </summary>
    /// <seealso cref="Serenity.ICriteria" />
    [DebuggerDisplay("{ToStringIgnoreParams()}")]
    [JsonConverter(typeof(JsonCriteriaConverter))]
    public abstract class BaseCriteria : ICriteria
    {
        private static NoParamsChecker noParamsChecker = new NoParamsChecker();
        private static IgnoreParams ignoreParams = new IgnoreParams();

        /// <summary>
        /// Gets a value indicating whether this criteria instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsEmpty
        {
            get { return false; }
        }

        /// <summary>
        /// Creates a new unary IsNull criteria containing this criteria as the operand.
        /// </summary>
        /// <returns></returns>
        public BaseCriteria IsNull()
        {
            return new UnaryCriteria(CriteriaOperator.IsNull, this);
        }

        /// <summary> 
        /// Creates a new unary IsNotNull criteria containing this criteria as the operand.
        /// </summary>
        /// <returns></returns>
        public BaseCriteria IsNotNull()
        {
            return new UnaryCriteria(CriteriaOperator.IsNotNull, this);
        }

        /// <summary>
        /// Creates a new binary Like criteria containing this criteria as the left operand.
        /// </summary>
        /// <param name="mask">The LIKE mask.</param>
        /// <returns></returns>
        public BaseCriteria Like(string mask)
        {
            return new BinaryCriteria(this, CriteriaOperator.Like, new ValueCriteria(mask));
        }

        /// <summary>
        /// Creates a new binary Not Like criteria containing this criteria as the left operand.
        /// </summary>
        /// <param name="mask">The like mask.</param>
        /// <returns></returns>
        public BaseCriteria NotLike(string mask)
        {
            return new BinaryCriteria(this, CriteriaOperator.NotLike, new ValueCriteria(mask));
        }

        /// <summary>
        /// Creates a new binary Stars With (LIKE '...%') criteria containing this criteria as the left operand.
        /// </summary>
        /// <param name="mask">The starts with mask.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">mask is null</exception>
        public BaseCriteria StartsWith(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            return Like(mask + "%");
        }

        /// <summary>
        /// Creates a new binary Ends With (LIKE '%...') criteria containing this criteria as the left operand.
        /// </summary>
        /// <param name="mask">The ends with mask.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">mask is null</exception>
        public BaseCriteria EndsWith(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            return Like("%" + mask);
        }

        /// <summary>
        /// Creates a new binary Contains criteria (LIKE '%...%') containing this criteria as the left operand.
        /// </summary>
        /// <param name="mask">The contains mask.</param>
        /// <returns></returns>
        public BaseCriteria Contains(string mask)
        {
            return Like("%" + mask + "%");
        }

        /// <summary>
        /// Creates a new binary Not Contains criteria (NOT LIKE '%...%') containing this criteria as the left operand.
        /// </summary>
        /// <param name="mask">The contains mask.</param>
        /// <returns></returns>
        public BaseCriteria NotContains(string mask)
        {
            return NotLike("%" + mask + "%");
        }

        public BaseCriteria In<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            if (values.Length == 1 &&
                values[0] is BaseCriteria)
            {
                return In((BaseCriteria)(object)values[0]);
            }

            if (values.Length == 1 &&
                !(values[0] is string) &&
                values[0] is IEnumerable)
            {
                return new BinaryCriteria(this, CriteriaOperator.In, new ValueCriteria(values[0]));
            }

            if (values.Length == 1 &&
                values[0] is ISqlQuery)
            {
                return In((ISqlQuery)(object)values[0]);
            }

            return new BinaryCriteria(this, CriteriaOperator.In, new ValueCriteria(values));
        }

        public BaseCriteria In(BaseCriteria statement)
        {
            if (Object.ReferenceEquals(null, statement) || statement.IsEmpty)
                throw new ArgumentNullException("statement");

            return new BinaryCriteria(this, CriteriaOperator.In, statement); 
        }

        public BaseCriteria InStatement(BaseCriteria statement)
        {
            return In(statement);
        }

        public BaseCriteria In(ISqlQuery statement)
        {
            if (Object.ReferenceEquals(null, statement))
                throw new ArgumentNullException("statement");

            return new BinaryCriteria(this, CriteriaOperator.In, new Criteria(statement));
        }

        public BaseCriteria NotIn<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            if (values.Length == 1 &&
                values[0] is BaseCriteria)
            {
                return NotIn((BaseCriteria)(object)values[0]);
            }

            if (values.Length == 1 &&
                !(values[0] is string) &&
                values[0] is IEnumerable)
            {
                return new BinaryCriteria(this, CriteriaOperator.NotIn, new ValueCriteria(values[0]));
            }

            if (values.Length == 1 &&
                values[0] is ISqlQuery)
            {
                return NotIn((ISqlQuery)(object)values[0]);
            }

            return new BinaryCriteria(this, CriteriaOperator.NotIn, new ValueCriteria(values));
        }

        public BaseCriteria NotIn(BaseCriteria statement)
        {
            if (Object.ReferenceEquals(null, statement) || statement.IsEmpty)
                throw new ArgumentNullException("statement");

            return new BinaryCriteria(this, CriteriaOperator.NotIn, statement);
        }

        public BaseCriteria NotIn(ISqlQuery statement)
        {
            if (Object.ReferenceEquals(null, statement))
                throw new ArgumentNullException("statement");

            return new BinaryCriteria(this, CriteriaOperator.NotIn, new Criteria(statement));
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

        public static BaseCriteria operator ==(BaseCriteria criteria1, Enum value)
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

        public static BaseCriteria operator !=(BaseCriteria criteria1, Enum value)
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

        public static BaseCriteria operator >(BaseCriteria criteria1, Enum value)
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

        public static BaseCriteria operator >=(BaseCriteria criteria1, Enum value)
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

        public static BaseCriteria operator <(BaseCriteria criteria1, Enum value)
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

        public static BaseCriteria operator <=(BaseCriteria criteria1, Enum value)
        {
            return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
        }

        private static BaseCriteria JoinIf(BaseCriteria criteria1, BaseCriteria criteria2, CriteriaOperator op)
        {
            if (ReferenceEquals(null, criteria1) || criteria1.IsEmpty)
                return criteria2;

            if (ReferenceEquals(null, criteria2) || criteria2.IsEmpty)
                return criteria1;

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
            return criteria;
        }

        /// <summary>
        /// Must return FALSE from this for short circuit OR (||) to return 
        /// a new binary criteria merging left and right operands in any case
        /// </summary>
        public static bool operator false(BaseCriteria statement)
        {
            return false;
        }

        /// <summary>
        /// Must ALSO return FALSE from this for short circuit AND (&amp;&amp;) to return 
        /// a new binary criteria merging left and right operands in any case
        /// https://msdn.microsoft.com/en-us/library/aa691312
        /// </summary>
        public static bool operator true(BaseCriteria statement)
        {
            return false;
        }

        /// <summary>
        /// Must override this or will get operator overload warning.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Must override this or will get operator overload warning.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>True if equals to object</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Converts the criteria to string while ignoring its params if any.
        /// ToString() raises an exception if a criteria has params, while this not.
        /// </summary>
        /// <returns></returns>
        public string ToStringIgnoreParams()
        {
            return ToString(ignoreParams);
        }

        /// <summary>
        /// Converts the criteria to string representation while adding params to the target query.
        /// </summary>
        /// <param name="query">The target query to add params to.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IQueryWithParams query)
        {
            var sb = new StringBuilder(256);
            ToString(sb, query);
            return sb.ToString();
        }

        /// <summary>
        /// Converts the criteria to string. Raises an exception if
        /// criteria contains parameters.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(noParamsChecker);
        }

        /// <summary>
        /// Converts the criteria to string representation into a string builder, while adding
        /// its params to the target query.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="query">The target query to add params to.</param>
        public virtual void ToString(StringBuilder sb, IQueryWithParams query)
        {
            throw new NotImplementedException();
        }

        private class NoParamsChecker : IQueryWithParams
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

            public ISqlDialect Dialect
            {
                get { return SqlSettings.DefaultDialect; }
            }
        }

        private class IgnoreParams : IQueryWithParams
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

            public ISqlDialect Dialect
            {
                get { return SqlSettings.DefaultDialect; }
            }
        }
    }
}
