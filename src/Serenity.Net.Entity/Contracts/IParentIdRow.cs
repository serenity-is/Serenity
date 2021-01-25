
namespace Serenity.Data
{
    /// <summary>
    /// IParentIdRow
    /// </summary>
    /// <seealso cref="Serenity.Data.IRow" />
    public interface IParentIdRow : IRow
    {
        /// <summary>
        /// Gets the parent identifier field.
        /// </summary>
        /// <value>
        /// The parent identifier field.
        /// </value>
        Field ParentIdField { get; }
    }
}
