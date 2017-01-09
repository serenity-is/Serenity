namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ConstantCriteria : Criteria
    {
        public ConstantCriteria(int value)
            : base(value.ToInvariant())
        {
        }

        public ConstantCriteria(IEnumerable<int> values)
            : base(String.Join(",", values))
        {
        }

        public ConstantCriteria(long value)
            : base(value.ToInvariant())
        {
        }

        public ConstantCriteria(IEnumerable<long> values)
            : base(String.Join(",", values))
        {
        }

        public ConstantCriteria(string value, ISqlDialect dialect = null)
            : base(value.ToSql(dialect))
        {
        }

        public ConstantCriteria(IEnumerable<string> values, ISqlDialect dialect = null)
            : base(String.Join(",", values.Select(x => x.ToSql(dialect))))
        {
        }
    }
}