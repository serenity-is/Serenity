namespace Serenity.Services;

/// <summary>
/// Marker interface for undelete behaviors that can be used as a mixin
/// within a UndeleteRequestHandler lifecycle.
/// </summary>
/// <remarks>
/// All undelete behaviors should implement either <see cref="IUndeleteBehaviorSync"/>
/// or <see cref="IUndeleteBehaviorAsync"/>, not this interface directly.
/// </remarks>
public interface IUndeleteBehavior
{
}
