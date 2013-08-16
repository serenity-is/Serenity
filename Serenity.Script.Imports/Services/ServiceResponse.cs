using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class ServiceResponse
    {
        public ServiceError Error { get; set; }
    }
}