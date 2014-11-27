namespace Serenity.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
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

        public ConstantCriteria(string value)
            : base(value.ToSql())
        {
        }

        public ConstantCriteria(IEnumerable<string> values)
            : base(String.Join(",", values.Select(x => x.ToSql())))
        {
        }
    }
}