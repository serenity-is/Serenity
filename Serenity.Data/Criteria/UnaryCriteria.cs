namespace Serenity.Data
{
    using System;
    using System.Text;

    public class UnaryCriteria : BaseCriteria
    {
        private CriteriaOperator op;
        private BaseCriteria operand;

        public UnaryCriteria(CriteriaOperator op, BaseCriteria operand)
        {
            if (Object.ReferenceEquals(operand, null))
                throw new ArgumentNullException("operand");

            if (op < CriteriaOperator.Paren || op > CriteriaOperator.Exists)
                throw new ArgumentOutOfRangeException("op");

            this.op = op;
            this.operand = operand;
        }

        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            switch (this.op)
            {
                case CriteriaOperator.Paren:
                    sb.Append('(');
                    this.operand.ToString(sb, query);
                    sb.Append(')');
                    break;

                case CriteriaOperator.Not:
                    sb.Append("NOT (");
                    this.operand.ToString(sb, query);
                    sb.Append(')');
                    break;

                case CriteriaOperator.IsNull:
                    this.operand.ToString(sb, query);
                    sb.Append(" IS NULL");
                    break;

                case CriteriaOperator.IsNotNull:
                    this.operand.ToString(sb, query);
                    sb.Append(" IS NOT NULL");
                    break;

                case CriteriaOperator.Exists:
                    sb.Append("EXISTS (");
                    this.operand.ToString(sb, query);
                    sb.Append(')');
                    break;
            }
        }

        public CriteriaOperator Operator
        {
            get { return op; }
        }

        public BaseCriteria Operand
        {
            get { return operand; }
        }
    }
}