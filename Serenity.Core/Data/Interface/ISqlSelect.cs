using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
    public interface ISqlSelect : ISqlQuery
    {
        void GetFromReader(IDataReader reader);
        void GetFromReader(IDataReader reader, IList<Row> into);
        bool CountRecords { get; }
        SqlDialect Dialect { get; }
    }
}