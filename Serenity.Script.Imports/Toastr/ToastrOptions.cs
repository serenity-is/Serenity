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
        public string PositionClass { get; set; }
        public int TimeOut { get; set; }
        public int FadeIn { get; set; }
        public int FadeOut { get; set; }
        public int ExtendedTimeOut { get; set; }
    }
}