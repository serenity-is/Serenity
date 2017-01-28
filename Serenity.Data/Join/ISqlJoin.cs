
using System;

namespace Serenity.Data.Mapping
{
    public interface ISqlJoin
    {
        string Alias { get; }
        string ToTable { get; }
        string OnCriteria { get; }
        string PropertyPrefix { get; }
        string TitlePrefix { get; set; }
        Type RowType { get; set; }
    }
}