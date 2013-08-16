using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serenity.Data;

namespace Serenity.Services
{
    public interface IIncludeExcludeColumns
    {
        HashSet<string> IncludeColumns { get; set; }
        HashSet<string> ExcludeColumns { get; set; }
    }

    public class ListRequest : ServiceRequest, IIncludeExcludeColumns
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public SortBy[] Sort { get; set; }
        public string ContainsText { get; set; }
        public List<FilterLine> FilterLines { get; set; }
        [JsonIgnore]
        public BasicFilter Filter { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool ExcludeTotalCount { get; set; }
        public ColumnSelection ColumnSelection { get; set; }
        [JsonConverter(typeof(JsonIdentifierSetConverter))]
        public HashSet<string> IncludeColumns { get; set; }
        [JsonConverter(typeof(JsonIdentifierSetConverter))]
        public HashSet<string> ExcludeColumns { get; set; }
    }

    public enum ColumnSelection
    {
        List = 0,
        KeyOnly = 1,
        Lookup = 2,
        Details = 3,
    }

    [Flags]
    public enum ListRequestSupport
    {
        Paging = 1,
        Sorting = 2,
        ContainsText = 4,
        IncludeDeleted = 8,
        FilterLines = 16,
        Filter = 32,
        SetPageSortContains = Paging | Sorting | ContainsText,
        SetFull = SetPageSortContains | Filter | FilterLines | IncludeDeleted
    }
}