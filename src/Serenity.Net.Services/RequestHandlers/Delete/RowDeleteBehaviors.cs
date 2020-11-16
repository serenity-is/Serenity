#if TODO
using Serenity.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Stores a static, cached list of delete behaviors that are implicitly or explicitly activated
    /// for a row type (TRow).
    /// </summary>
    /// <typeparam name="TRow">Type of row</typeparam>
    public class RowDeleteBehaviors<TRow> : RowBehaviors<TRow, IDeleteBehavior>
        where TRow: class, IRow, new()
    {
    }
}
#endif