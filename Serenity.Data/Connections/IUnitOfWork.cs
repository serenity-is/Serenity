using System.Data;
using System;

namespace Serenity.Data
{
    public interface IUnitOfWork
    {
        IDbConnection Connection { get; }
        event Action OnCommit;
        event Action OnRollback;
    }
}