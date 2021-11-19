using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies SQL expression this property corresponds to.
    /// You may use brackets ([]) to escape identifiers. Brackets will be converted to database specific quotes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExpressionAttribute : Attribute
    {
        /// <summary>
        /// Specifies SQL expression this property corresponds to.
        /// </summary>
        /// <param name="value">An SQL expression like (T0.Firstname + ' ' + T0.LastName)</param>
        public ExpressionAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Specifies SQL expression and dialects this property corresponds to.
        /// </summary>
        /// <param name="value">An SQL expression like (T0.Firstname + ' ' + T0.LastName)</param>
        /// <param name="serverTypes">Dialects like <see cref="ServerType.MySql" />, <see cref="ServerType.Sqlite" />.</param>
        public ExpressionAttribute(string value, params ServerType[] serverTypes)
            : this(value)
        {
            Dialect = string.Join(",", serverTypes);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets or sets the dialect.
        /// </summary>
        /// <value>
        /// The dialect.
        /// </value>
        public string Dialect { get; set; }

        /// <summary>
        /// Gets or sets the negating of the dialect.
        /// </summary>
        /// <value>
        /// The negating of the dialect.
        /// </value>
        public bool NegateDialect
        {
            get => Dialect != null && Dialect.StartsWith('!');
            set => Dialect = value ? (!NegateDialect ? ("!" + Dialect) : Dialect) :
                (NegateDialect ?  Dialect[1..] : Dialect);
        }
    }
}