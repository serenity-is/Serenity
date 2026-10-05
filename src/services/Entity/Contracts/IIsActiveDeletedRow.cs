
namespace Serenity.Data;

/// <summary>
/// Marks an <see cref="IIsActiveRow"/> where -1 is used as the deleted value.
/// </summary>
/// <remarks>
/// Because the deleted value (<c>-1</c>) is stored in the same column as the active (<c>1</c>) and inactive
/// (<c>0</c>) values, the state a row had before deletion is lost. Undelete always restores <c>1</c>, so a row
/// that was inactive before being deleted becomes active afterwards. Preserving it would require storing the
/// prior state separately at delete time.
/// </remarks>
public interface IIsActiveDeletedRow : IIsActiveRow
{
}
