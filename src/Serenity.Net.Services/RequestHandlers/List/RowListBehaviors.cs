#if TODO
using Serenity.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Stores a static, cached list of list behaviors that are implicitly or explicitly activated
    /// for a row type (TRow).
    /// </summary>
    /// <typeparam name="TRow">Type of row</typeparam>
    public class RowListBehaviors<TRow> : RowBehaviors<TRow, IListBehavior>
        where TRow: class, IRow, new()
    {
    }
}
#endif