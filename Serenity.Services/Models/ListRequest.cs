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
        public string ContainsField { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool ExcludeTotalCount { get; set; }
        public Dictionary<string, object> EqualityFilter { get; set; }
        public ColumnSelection ColumnSelection { get; set; }
        [JsonConverter(typeof(JsonStringHashSetConverter))]
        public HashSet<string> IncludeColumns { get; set; }
        [JsonConverter(typeof(JsonStringHashSetConverter))]
        public HashSet<string> ExcludeColumns { get; set; }
    }

    public enum ColumnSelection
    {
        List = 0,
        KeyOnly = 1,
        Details = 2,
    }

    //[Flags] //TODO: add this validation
    //public enum ListRequestSupport
    //{
    //    Paging = 1,
    //    Sorting = 2,
    //    ContainsText = 4,
    //    IncludeDeleted = 8
    //    FilterLines = 16,
    //    Filter = 32,
    //    SetPageSortContains = Paging | Sorting | ContainsText,
    //    SetFull = SetPageSortContains | Filter | FilterLines | IncludeDeleted
    //}
}