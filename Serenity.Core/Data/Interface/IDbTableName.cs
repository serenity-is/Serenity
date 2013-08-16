using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    /// <summary>
    ///   Interface for objects that is related to a table</summary>
    public interface IDbTableName
    {
        /// <summary>
        ///   Returns table name. Each object that is related to a table should 
        ///   have a "Table" property.</summary>
        string Table { get; }
    }
}
