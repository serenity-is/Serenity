namespace Serenity.Data
{
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
                        if (k is int ||
                            k is short ||
                            k is uint ||
                            k is long ||
                            k is byte)
                        {
                            sb.Append(k.ToString());
                            continue;
                        }
                    }
                    var param = query.AutoParam();
                    query.AddParam(param.Name, k);
                    sb.Append(param.Name);
                }
                sb.Append(')');
            }
            else
            {
                var param = query.AutoParam();
                query.AddParam(param.Name, this.value);
                sb.Append(param.Name);
            }
        }
    }
}