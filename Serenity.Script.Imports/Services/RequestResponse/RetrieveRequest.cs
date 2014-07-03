using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class RetrieveRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
        public ColumnSelection ColumnSelection { get; set; }
        public List<string> IncludeColumns { get; set; }
        public List<string> ExcludeColumns { get; set; }
    }
}