using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class SaveResponse : ServiceResponse
    {
        public object EntityId { get; set; }
    }
}