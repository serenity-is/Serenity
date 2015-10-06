namespace Serenity
{
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    [ScriptName("Criteria")]
    public static class CriteriaUtil
    {
        public static bool IsEmpty(BaseCriteria criteria)
        {
            var array = criteria.As<object[]>();

            return array.Length == 0 ||
                (array.Length == 1 && array[0] is string && ((string)array[0]).Length == 0);
        }

        public static BaseCriteria Join(BaseCriteria criteria1, string op, BaseCriteria criteria2)
        {
            if (ReferenceEquals(null, criteria1))
                throw new ArgumentNullException("criteria1");

            if (ReferenceEquals(null, criteria2))
                throw new ArgumentNullException("criteria2");

            if (criteria1.IsEmpty)
                return criteria2;

            if (criteria2.IsEmpty)
                return criteria1;

            return new BinaryCriteria(criteria1, op, criteria2);
        }

        public static BaseCriteria Paren(BaseCriteria criteria)
        {
            if (!criteria.IsEmpty)
                return new UnaryCriteria("()", criteria);

            return criteria;
        }
    }
}