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
            if (ReferenceEquals(left, null))
                throw new ArgumentNullException("left");

            if (ReferenceEquals(right, null))
                throw new ArgumentNullException("right");

                throw new ArgumentOutOfRangeException("op");

            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            if (this.op == CriteriaOperator.Like ||
                this.op == CriteriaOperator.NotLike)
            {
                var valueCriteria = this.right as ValueCriteria;
                if (query.Dialect.IsLikeCaseSensitive &&
                    !ReferenceEquals(null, valueCriteria) &&
                    valueCriteria.Value is string)
                {
                    sb.Append("UPPER(");
                    this.left.ToString(sb, query);
                    sb.Append(this.op == CriteriaOperator.Like ? ") LIKE UPPER(" : ") NOT LIKE UPPER(");
                    this.right.ToString(sb, query);
                    sb.Append(")");
                }
                else
                {
                    this.left.ToString(sb, query);
                    sb.Append(this.op == CriteriaOperator.Like ? " LIKE " : " NOT LIKE ");
                    this.right.ToString(sb, query);
                }
            }
            else if (this.op == CriteriaOperator.FullTextSearchContains)
            {
                // Full-text search queries are case-insensitive
                sb.Append("CONTAINS(");
                this.left.ToString(sb, query);
                sb.Append(',');
                this.right.ToString(sb, query);
                sb.Append(")");
            }
            else
            {
                sb.Append('(');
                this.left.ToString(sb, query);
                sb.Append(opText[(int)this.op - (int)CriteriaOperator.AND]);
                this.right.ToString(sb, query);
                sb.Append(')');
            }
        }

        private static readonly string[] opText =
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

        public CriteriaOperator Operator
        {
            get { return op; }
        }

        public BaseCriteria LeftOperand
        {
            get { return left; }
        }

        public BaseCriteria RightOperand
        {
            get { return right; }
        }
    }
}