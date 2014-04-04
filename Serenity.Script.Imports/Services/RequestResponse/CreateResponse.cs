using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class CreateResponse : ServiceResponse
    {
        public Int64? EntityId { get; set; }
    }
}