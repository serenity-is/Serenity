
namespace Serenity.Data
{
    public interface ILocalizationRow : IIdRow
    {
        Field CultureIdField { get; }
    }
}