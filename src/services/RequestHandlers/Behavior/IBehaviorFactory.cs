namespace Serenity.Services;

/// <summary>
/// Behavior factory abstraction
/// </summary>
public interface IBehaviorFactory
{
    /// <summary>
    /// Creates an instance of the behavior type
    /// </summary>
    /// <param name="behaviorType">The behavior type</param>
    object CreateInstance(Type behaviorType);
}