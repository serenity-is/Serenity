using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class DeleteRequest : ServiceRequest
    {
        public object EntityId { get; set; }
    }
}