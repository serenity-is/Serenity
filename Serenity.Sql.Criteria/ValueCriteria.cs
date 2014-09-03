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
                int i = 0;
                sb.Append('(');
                foreach (var k in enumerable)
                {
                    var param = query.AutoParam();
                    query.AddParam(param.Name, k);
                    if (i++ > 0)
                        sb.Append(',');
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