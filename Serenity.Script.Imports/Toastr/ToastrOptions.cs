using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the jQuery toastr plugin
    /// </summary>
    [Imported, Serializable]
    public class ToastrOptions
    {
        public object Target { get; set; }
        public string ContainerId { get; set; }
        public string PositionClass { get; set; }
        public int TimeOut { get; set; }
        public int ShowDuration { get; set; }
        public int HideDuration { get; set; }
        public int ExtendedTimeOut { get; set; }
    }
}