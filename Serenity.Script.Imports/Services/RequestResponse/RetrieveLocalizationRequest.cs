using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class RetrieveLocalizationRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
        public string CultureId { get; set; }
    }
}