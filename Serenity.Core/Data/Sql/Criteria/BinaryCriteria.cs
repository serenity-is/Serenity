namespace Serenity.Data
{
    using System;
    using System.Text;

    public class BinaryCriteria : BaseCriteria
    {
        private BaseCriteria left;
        private BaseCriteria right;
        private CriteriaOperator op;

        public BinaryCriteria(BaseCriteria left, CriteriaOperator op, BaseCriteria right)
        {
            if (Object.ReferenceEquals(left, null))
                throw new ArgumentNullException("left");

            if (Object.ReferenceEquals(right, null))
                throw new ArgumentNullException("right");

            if (op < CriteriaOperator.AND || op > CriteriaOperator.Like)
                throw new ArgumentOutOfRangeException("op");

            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override void ToString(StringBuilder sb, IDbParameterized query)
        {
            if (this.op == CriteriaOperator.Like ||
                this.op == CriteriaOperator.NotLike)
            {
                var valueCriteria = this.right as ValueCriteria;
                if (SqlSettings.CurrentDialect.IsCaseSensitive() &&
                    !Object.ReferenceEquals(null, valueCriteria) &&
                    valueCriteria.Value is string)
                {
                    var mask = ((string)valueCriteria.Value).ToUpper();
                    sb.Append("UPPER(");
                    this.left.ToString(sb, query);
                    sb.Append(this.op == CriteriaOperator.Like ? ") LIKE " : ") NOT LIKE ");
                    this.right.ToString(sb, query);
                }
                else
                {
                    this.left.ToString(sb, query);
                    sb.Append(this.op == CriteriaOperator.Like ? " LIKE " : " NOT LIKE ");
                    this.right.ToString(sb, query);
                }
            }
            else
            {
                this.left.ToString(sb, query);
                sb.Append(opText[(int)this.op - (int)CriteriaOperator.AND]);
                this.right.ToString(sb, query);
            }
        }

        private static string[] opText = new string[]
        {
            " AND ",
            " OR ",
            " XOR ",
            " = ",
            " != ",
            " > ",
            " >= ",
            " < ",
            " <= ",
            " IN ",
            " NOT IN "
        };
    }
}