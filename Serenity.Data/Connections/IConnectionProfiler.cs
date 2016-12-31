using System.Data;

namespace Serenity.Data
{
    public interface IConnectionProfiler
    {
        IDbConnection Profile(IDbConnection connection);
    }
}