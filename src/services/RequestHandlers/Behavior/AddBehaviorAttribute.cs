namespace Serenity.ComponentModel;

/// <summary>
/// Attaches an explicit behavior to a class or property.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class AddBehaviorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the attribute.
    /// </summary>
    /// <param name="behaviorType">Behavior type</param>
    /// <exception cref="ArgumentException"><paramref name="behaviorType"/> is abstract or an interface.</exception>
    public AddBehaviorAttribute(Type behaviorType)
    {
        if (behaviorType.IsAbstract || behaviorType.IsInterface)
            throw new ArgumentException("Behavior type cannot be abstract or interface!");
        
        Value = behaviorType;
    }

    /// <summary>
    /// Gets the behavior type.
    /// </summary>
    public Type Value { get; private set; }
}