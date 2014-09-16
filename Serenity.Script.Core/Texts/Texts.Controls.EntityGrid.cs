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
            public static class EntityGrid
            {
                public static LocalText NewButton = "New {0}";
                public static LocalText RefreshButton = "Refresh";

                static EntityGrid()
                {
                    LocalText.InitializeTextClass(typeof(EntityGrid), "Controls.EntityGrid.");
                }
            }
        }
    }
}