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
            public static class QuickSearch
            {
                public static LocalText Placeholder = "search...";
                public static LocalText Hint = "enter the text to search for...";
                public static LocalText FieldSelection = "select the field to search on";

                static QuickSearch()
                {
                    LocalText.InitializeTextClass(typeof(QuickSearch), "Controls.QuickSearch.");
                }
            }
        }
    }
}