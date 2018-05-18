using System;

namespace Serenity.Extensibility
{
    /// <summary>
    /// Base class for attributes attached to an assembly that should run static constructor
    /// of a type at app start.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public abstract class BaseRegistrarAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRegistrarAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public BaseRegistrarAttribute(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; private set; }
    }
}