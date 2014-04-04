using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class RetrieveRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
    }
}