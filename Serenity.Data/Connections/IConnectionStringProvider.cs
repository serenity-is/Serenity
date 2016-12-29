using System.Configuration;

namespace Serenity.Data
{
    interface IConnectionStringProvider
    {
        ConnectionStringSettings GetConnectionString(string connectionKey);
    }
}
