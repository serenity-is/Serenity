using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class RetrieveRequest : ServiceRequest
    {
        public object EntityId { get; set; }
        public RetrieveColumnSelection ColumnSelection { get; set; }
        public List<string> IncludeColumns { get; set; }
        public List<string> ExcludeColumns { get; set; }
    }

    [NamedValues]
    public enum RetrieveColumnSelection
    {
        Details = 0,
        KeyOnly = 1,
        List = 2
    }
}