
namespace Serenity.Data
{
    public interface INameRow
    {
        /// <summary>
        ///   Gets name field for this row (usually contains a local text key).</summary>
        StringField NameField { get; }
    }
}