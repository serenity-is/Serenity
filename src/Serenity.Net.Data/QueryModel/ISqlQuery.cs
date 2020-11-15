
namespace Serenity.Data
{
    /// <summary>
    /// SqlQuery interface.
    /// </summary>
    /// <seealso cref="Serenity.Data.IQueryWithParams" />
    /// <seealso cref="Serenity.IChainable" />
    public interface ISqlQuery : IQueryWithParams, IChainable
    {
    }
}
