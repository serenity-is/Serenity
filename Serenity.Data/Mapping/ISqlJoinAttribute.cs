
using System;

namespace Serenity.Data.Mapping
{
    public interface ISqlJoin
    {
        string Alias { get; }
        string ToTable { get; }
        string OnCriteria { get; }
        string Prefix { get; }
        Type RowType { get; }
    }
}