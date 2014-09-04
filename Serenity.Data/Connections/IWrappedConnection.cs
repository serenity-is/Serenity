using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serenity.Data
{
    public interface IWrappedConnection : IDbConnection
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
    }
}