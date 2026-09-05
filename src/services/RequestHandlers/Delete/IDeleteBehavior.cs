namespace Serenity.Services;

/// <summary>
/// Marker interface for delete behaviors that can be used as a mixin
/// within a DeleteRequestHandler lifecycle.
/// </summary>
/// <remarks>
/// All delete behaviors should implement either <see cref="IDeleteBehaviorSync"/>
/// or <see cref="IDeleteBehaviorAsync"/>, not this interface directly.
/// </remarks>
public interface IDeleteBehavior
{
}
