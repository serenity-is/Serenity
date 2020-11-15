using Serenity.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Stores a static, cached list of retrieve behaviors that are implicitly or explicitly activated
    /// for a row type (TRow).
    /// </summary>
    /// <typeparam name="TRow">Type of row</typeparam>
    public class RowRetrieveBehaviors<TRow> : RowBehaviors<TRow, IRetrieveBehavior>
        where TRow: Row, new()
    {
    }
}