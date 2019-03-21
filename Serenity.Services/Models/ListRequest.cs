using System.Collections.Generic;
using Newtonsoft.Json;
using Serenity.Data;

namespace Serenity.Services
{
    public class ListRequest : ServiceRequest, IIncludeExcludeColumns
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public SortBy[] Sort { get; set; }
        public string ContainsText { get; set; }
        public string ContainsField { get; set; }
        [JsonConverter(typeof(JsonSafeCriteriaConverter))]
        public BaseCriteria Criteria { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool ExcludeTotalCount { get; set; }
        public Dictionary<string, object> EqualityFilter { get; set; }
        public ColumnSelection ColumnSelection { get; set; }
        [JsonConverter(typeof(JsonStringHashSetConverter))]
        public HashSet<string> IncludeColumns { get; set; }
        [JsonConverter(typeof(JsonStringHashSetConverter))]
        public HashSet<string> ExcludeColumns { get; set; }
        public SortBy[] DistinctFields { get; set; }
    }
}