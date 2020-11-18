namespace Serenity.Data
{
    using System;
    using System.Text;

    /// <summary>
    /// Unary criteria with one operand and operator
    /// </summary>
    /// <seealso cref="Serenity.Data.BaseCriteria" />
    public class UnaryCriteria : BaseCriteria
    {
        private readonly CriteriaOperator op;
        private readonly BaseCriteria operand;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryCriteria"/> class.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <param name="operand">The operand.</param>
        /// <exception cref="ArgumentNullException">operand</exception>
        /// <exception cref="ArgumentOutOfRangeException">operator</exception>
        public UnaryCriteria(CriteriaOperator op, BaseCriteria operand)
        {
            if (operand is null)
                throw new ArgumentNullException("operand");

            if (op < CriteriaOperator.Paren || op > CriteriaOperator.Exists)
                throw new ArgumentOutOfRangeException("op");

            this.op = op;
            this.operand = operand;
        }

        /// <summary>
        /// Converts the criteria to string.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="query">The target query to add params to.</param>
        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            switch (op)
            {
                case CriteriaOperator.Paren:
                    sb.Append('(');
                    operand.ToString(sb, query);
                    sb.Append(')');
                    break;

                case CriteriaOperator.Not:
                    sb.Append("NOT (");
                    operand.ToString(sb, query);
                    sb.Append(')');
                    break;

                case CriteriaOperator.IsNull:
                    operand.ToString(sb, query);
                    sb.Append(" IS NULL");
                    break;

                case CriteriaOperator.IsNotNull:
                    operand.ToString(sb, query);
                    sb.Append(" IS NOT NULL");
                    break;

                case CriteriaOperator.Exists:
                    sb.Append("EXISTS (");
                    operand.ToString(sb, query);
                    sb.Append(')');
                    break;
            }
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public CriteriaOperator Operator
        {
            get { return op; }
        }

        /// <summary>
        /// Gets the operand.
        /// </summary>
        /// <value>
        /// The operand.
        /// </value>
        public BaseCriteria Operand
        {
            get { return operand; }
        }
    }
}