namespace Serenity.Services;

/// <summary>
/// Marker interface for retrieve behaviors that can be used as a mixin
/// within a RetrieveRequestHandler lifecycle.
/// </summary>
/// <remarks>
/// All retrieve behaviors should implement either <see cref="IRetrieveBehaviorSync"/>
/// or <see cref="IRetrieveBehaviorAsync"/>, not this interface directly.
/// </remarks>
public interface IRetrieveBehavior
{
}
