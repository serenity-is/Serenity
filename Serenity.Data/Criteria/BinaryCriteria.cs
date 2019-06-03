namespace Serenity.Data
{
    using System;
    using System.Text;

    /// <summary>
    /// Binary criteria object, which has two operands and a operator.
    /// </summary>
    /// <seealso cref="Serenity.Data.BaseCriteria" />
    public class BinaryCriteria : BaseCriteria
    {
        private BaseCriteria left;
        private BaseCriteria right;
        private CriteriaOperator op;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryCriteria"/> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="op">The operator.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="ArgumentNullException">
        /// Left or right operand is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Operator is not a binary one.</exception>
        public BinaryCriteria(BaseCriteria left, CriteriaOperator op, BaseCriteria right)
        {
            if (ReferenceEquals(left, null))
                throw new ArgumentNullException("left");

            if (ReferenceEquals(right, null))
                throw new ArgumentNullException("right");

            if (op < CriteriaOperator.AND || op > CriteriaOperator.NotLike)
                throw new ArgumentOutOfRangeException("op");

            this.left = left;
            this.right = right;
            this.op = op;
        }

        /// <summary>
        /// Converts the criteria to string in a string builder, 
        /// while adding its params to the target query.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="query">The target query.</param>
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
        /// Gets the left operand.
        /// </summary>
        /// <value>
        /// The left operand.
        /// </value>
        public BaseCriteria LeftOperand
        {
            get { return left; }
        }

        /// <summary>
        /// Gets the right operand.
        /// </summary>
        /// <value>
        /// The right operand.
        /// </value>
        public BaseCriteria RightOperand
        {
            get { return right; }
        }
    }
}