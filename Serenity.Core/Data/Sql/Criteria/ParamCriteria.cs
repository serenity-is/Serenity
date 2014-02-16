namespace Serenity.Data
{
    using System;
    using System.Text;

    public class ParamCriteria : BaseCriteria
    {
        private object name;

        public ParamCriteria(string name)
        {
            if (name.IsEmptyOrNull())
                throw new ArgumentNullException("name");

            if (!name.StartsWith("@"))
                throw new ArgumentOutOfRangeException("name");

            this.name = name;
        }

        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            sb.Append(this.name);
        }
    }
}