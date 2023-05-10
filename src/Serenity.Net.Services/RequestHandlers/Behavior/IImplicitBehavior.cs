namespace Serenity.Services;

/// <summary>
/// An implicit behavior is a request handler behavior (save/list/retrieve/delete etc.)
/// which can be implicitly activated for a row type without explicitly attached 
/// to that row type.
/// </summary>
public interface IImplicitBehavior
{
    /// <summary>
    /// Returns true if this behavior should be used for the target row type.
    /// </summary>
    /// <param name="row">The target row type.</param>
    bool ActivateFor(IRow row);
}