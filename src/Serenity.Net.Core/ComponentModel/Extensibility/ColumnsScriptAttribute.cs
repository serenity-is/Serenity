using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that this type should generate a columns script, 
    /// which contains information about properties in this type and 
    /// is an array of PropertyItem objects. Column scripts can be
    /// accessed from client side using Q.getColumns("Key")
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ColumnsScriptAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnsScriptAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentNullException">key</exception>
        public ColumnsScriptAttribute(string key)
        {
            if (key.IsEmptyOrNull())
                throw new ArgumentNullException("key");

            Key = key;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }
    }
}