using System;

namespace Serenity.Extensibility
{
    /// <summary>
    /// Obsolete. Used on assembly to register classes with static constructors that 
    /// registers scripts to run at startup.
    /// </summary>
    /// <seealso cref="Serenity.Extensibility.BaseRegistrarAttribute" />
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    [Obsolete]
    public sealed class ScriptRegistrarAttribute : BaseRegistrarAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptRegistrarAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ScriptRegistrarAttribute(Type type)
            : base(type)
        {
        }
    }
}