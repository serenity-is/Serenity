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
            public static class Pager
            {
                public static LocalText Page = "Page";
                public static LocalText PageStatus = "Showing {from} to {to} of {total} total records";
                public static LocalText NoRowStatus = "No records";
                public static LocalText LoadingStatus = "Please wait, loading data...";
                public static LocalText DefaultLoadError = "An error occured while loading data!";

                static Pager()
                {
                    LocalText.InitializeTextClass(typeof(Pager), "Controls.Pager.");
                }
            }
        }
    }
}