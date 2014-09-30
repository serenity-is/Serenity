namespace Serenity.Data
{
    using System;
    using System.Text;

    public class ParamCriteria : BaseCriteria
    {
        private string name;

        public ParamCriteria(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (!name.StartsWith("@"))
                throw new ArgumentOutOfRangeException("name");

            this.name = name;
        }

        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            sb.Append(this.name);
        }

        public string Name
        {
            get { return name; }
        }
    }
}