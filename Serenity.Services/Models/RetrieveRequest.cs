using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serenity.Services
{
    public enum RetrieveColumnSelection
    {
        Details = 0,
        KeyOnly = 1,
        List = 2
    }

    public class RetrieveRequest : ServiceRequest, IIncludeExcludeColumns
    {
        public Int64? EntityId { get; set; }
        public RetrieveColumnSelection ColumnSelection { get; set; }
        [JsonConverter(typeof(JsonStringHashSetConverter))]
        public HashSet<string> IncludeColumns { get; set; }
        [JsonConverter(typeof(JsonStringHashSetConverter))]
        public HashSet<string> ExcludeColumns { get; set; }
    }
}