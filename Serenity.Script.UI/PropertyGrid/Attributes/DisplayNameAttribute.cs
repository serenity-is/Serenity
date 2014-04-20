using System;

namespace System.ComponentModel
{
    public class DisplayNameAttribute : Attribute
    {
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; private set; }
    }
}
