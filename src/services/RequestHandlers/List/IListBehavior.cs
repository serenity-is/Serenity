namespace Serenity.Services;

/// <summary>
/// Marker interface for list behaviors that can be used as a mixin
/// within a ListRequestHandler lifecycle.
/// </summary>
/// <remarks>
/// All list behaviors should implement either <see cref="IListBehaviorSync"/>
/// or <see cref="IListBehaviorAsync"/>, not this interface directly.
/// </remarks>
public interface IListBehavior
{
}
