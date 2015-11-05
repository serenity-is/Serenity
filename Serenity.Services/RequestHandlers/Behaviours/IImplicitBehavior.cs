using Serenity.Data;

namespace Serenity.Services
{
    /// <summary>
    /// An implicit behavior is a request handler behavior (save/list/retrieve/delete etc.)
    /// which can be implicitly activated for a row type without explicitly attached 
    /// to that row type.
    /// </summary>
    public interface IImplicitBehavior
    {
        bool ActivateFor(Row row);
    }
}