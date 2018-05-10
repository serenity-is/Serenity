
namespace Serenity.Data
{
    /// <summary>
    /// Marks a row as having a Name field (e.g. description for row).
    /// You may also use [NameProperty] instead (if your name field is not string)
    /// </summary>
    public interface INameRow
    {
        /// <summary>
        ///   Gets name field for this row.</summary>
        StringField NameField { get; }
    }
}