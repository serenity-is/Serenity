namespace Serenity.Data
{
    using System;
    using System.Collections;
    using System.Text;

    public class ValueCriteria : BaseCriteria
    {
        private object value;

        public ValueCriteria(object value)
        {
            this.value = value;
        }

        public object Value
        {
            get
            {
                return value;
            }
        }

        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            var enumerable = value as IEnumerable;
            if (enumerable != null && !(value is string))
            {
                var c = 0;
                foreach (var k in enumerable)
                    c++;

                int i = 0;
                sb.Append('(');
                foreach (var k in enumerable)
                {
                    if (i++ > 0)
                        sb.Append(',');

                    if (c > 10)
                    {
                        if (IsIntegerType(k))
                        {
                            sb.Append(k.ToString());
                            continue;
                        }
                        else if (k is Enum)
                        {
                            sb.Append(Convert.ToInt64(k).ToString());
                            continue;
                        }
                    }
                    sb.Append(AddParam(query, k).Name);
                }
                sb.Append(')');
            }
            else
            {
                sb.Append(AddParam(query, this.value).Name);
            }
        }

        private bool IsIntegerType(object k)
        {
            if (k == null)
                return false;

            if (k is int || k is long)
                return true;

            if (!k.GetType().GetIsPrimitive())
                return false;

            return k is Byte ||
                   k is SByte ||
                   k is Int16 ||
                   k is UInt16 ||
                   k is UInt32 ||
                   k is UInt64;
        }

        private Parameter AddParam(IQueryWithParams query, object value)
        {
            var param = query.AutoParam();
            query.AddParam(param.Name, value);
            return param;
        }
    }
}