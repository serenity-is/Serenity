using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Class that contains variables returned by a service in case of an error
    /// </summary>
    [Imported, Serializable, PreserveMemberCase]
    public class ServiceError
    {
        public string Code { get; set; }
        public string Arguments { get; set; }
        public string Message { get; set; }
    }
}