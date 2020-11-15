using System;

namespace Serenity.Data
{
    public interface IIdField
    {
        Int64? this[IRow row] { get; set; }
        bool IsIntegerType { get; }
    }
}
