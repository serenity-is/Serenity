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
            if (behaviorType.IsAbstract || behaviorType.IsInterface)
                throw new ArgumentException("Behavior type cannot be abstract or interface!");
            
            Value = behaviorType;
        }

        public Type Value { get; private set; }
    }
}