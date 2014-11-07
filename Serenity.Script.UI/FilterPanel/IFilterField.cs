
namespace Serenity
{
    public interface IFilterField
    {
        string Name { get; }
        string Title { get; }
        bool NotNull { get; }
        string FilteringType { get; }
    }
}