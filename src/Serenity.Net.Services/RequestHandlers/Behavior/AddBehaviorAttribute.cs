using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Attaches an explicit behavior
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class AddBehaviorAttribute : Attribute
    {
        public AddBehaviorAttribute(Type behaviorType)
        {
            Value = behaviorType;
        }

        public Type Value { get; private set; }
    }
}