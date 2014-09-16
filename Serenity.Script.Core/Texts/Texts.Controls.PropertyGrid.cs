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
                public static LocalText RequiredHint = "this field is required";

                static PropertyGrid()
                {
                    LocalText.InitializeTextClass(typeof(PropertyGrid), "Controls.PropertyGrid.");
                }
            }
        }
    }
}