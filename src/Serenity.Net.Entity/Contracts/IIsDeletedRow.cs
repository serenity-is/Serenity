
namespace Serenity.Data
{
    /// <summary>
    /// Determines that this row uses soft delete and the field that holds this flag
    /// </summary>
    public interface IIsDeletedRow
    {
        BooleanField IsDeletedField { get; }
    }
}
