using System.Data;
using System;

namespace Serenity.Data
{
    public interface IConnectionProfiler
    {
        IDbConnection Profile(IDbConnection connection);
    }
}