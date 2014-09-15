using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Texts
    {
        public static partial class Controls
        {
            [PreserveMemberCase]
            public static class PropertyGrid
            {
                public static LocalText DefaultCategory = "Properties";

                static PropertyGrid()
                {
                    LocalText.InitializeTextClass(typeof(PropertyGrid), "Controls.PropertyGrid.");
                }
            }
        }
    }
}