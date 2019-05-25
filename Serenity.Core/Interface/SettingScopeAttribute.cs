using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Determines scope of a setting like "Application", "Database", "Tenant" etc.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class SettingScopeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingScopeAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SettingScopeAttribute(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}