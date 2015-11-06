using Serenity.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Stores a static, cached list of save behaviors that are implicitly or explicitly activated
    /// for a row type (TRow).
    /// </summary>
    /// <typeparam name="TRow">Type of row</typeparam>
    public class RowSaveBehaviors<TRow> : RowBehaviors<TRow, ISaveBehavior>
        where TRow: Row, new()
    {
    }
}