
namespace Serenity.Data
{
    /// <summary>
    ///   Interface for aliases.</summary>
    public interface IAlias
    {
        string Name { get; }
        string NameDot { get; }
        string Table { get; }
    }
}
