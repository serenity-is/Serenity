using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class UndeleteRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
    }
}