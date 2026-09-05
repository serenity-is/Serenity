namespace Serenity.Services;

/// <summary>
/// Marker interface for save behaviors that can be used as a mixin
/// within a SaveRequestHandler lifecycle.
/// </summary>
/// <remarks>
/// All save behaviors should implement either <see cref="ISaveBehaviorSync"/>
/// or <see cref="ISaveBehaviorAsync"/>, not this interface directly.
/// </remarks>
public interface ISaveBehavior
{
}
