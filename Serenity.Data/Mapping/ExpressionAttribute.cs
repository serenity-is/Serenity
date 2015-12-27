using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies SQL expression this property corresponds to.
    /// You may use brackets ([]) to escape identifiers. Brackets will be converted to database specific quotes.
    /// </summary>
    public class ExpressionAttribute : Attribute
    {
        /// <summary>
        /// Specifies SQL expression this property corresponds to.
        /// </summary>
        /// <param name="value">An SQL expression like (T0.Firstname + ' ' + T0.LastName)</param>
        public ExpressionAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}