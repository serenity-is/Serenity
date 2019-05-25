using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Determines key for a setting type.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class SettingKeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingKeyAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SettingKeyAttribute(string value)
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