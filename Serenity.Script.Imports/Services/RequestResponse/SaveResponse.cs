using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class SaveResponse : ServiceResponse
    {
        public Int64? EntityId { get; set; }
    }
}