using System;

namespace Serenity.Data
{
    public interface IIdField
    {
        Int64? this[Row row] { get; set; }
    }
}
