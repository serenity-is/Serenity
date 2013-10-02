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
    }
}