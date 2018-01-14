using System;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
    [Imported(ObeysTypeSystem = true)]
    public class DisplayNameAttribute : Attribute
    {
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        [IntrinsicProperty]
        public string DisplayName { get; private set; }
    }
}
