namespace Serenity.ComponentModel;

/// <summary>
/// Attaches an explicit behavior
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class AddBehaviorAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of the attribute
    /// </summary>
    /// <param name="behaviorType">Behavior type</param>
    /// <exception cref="ArgumentException">behaviorType is abstract or an interface</exception>
    public AddBehaviorAttribute(Type behaviorType)
    {
        if (behaviorType.IsAbstract || behaviorType.IsInterface)
            throw new ArgumentException("Behavior type cannot be abstract or interface!");
        
        Value = behaviorType;
    }

    /// <summary>
    /// Gets the behavior type
    /// </summary>
    public Type Value { get; private set; }
}