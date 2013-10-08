namespace Serenity
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public class ListRequest : ServiceRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string[] Sort { get; set; }
        public string ContainsText { get; set; }
        public string ContainsField { get; set; }
        public List<object> FilterLines { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool ExcludeTotalCount { get; set; }
        public ColumnSelection ColumnSelection { get; set; }
        public List<string> IncludeColumns { get; set; }
        public List<string> ExcludeColumns { get; set; }
    }

    [NamedValues]
    public enum ColumnSelection
    {
        List = 0,
        KeyOnly = 1,
        Lookup = 2,
        Details = 3,
    }
}