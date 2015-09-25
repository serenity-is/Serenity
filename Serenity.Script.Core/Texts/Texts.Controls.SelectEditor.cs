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
            public static class SelectEditor
            {
                public static LocalText InplaceAdd = "Define New";
                public static LocalText InplaceEdit = "Edit";
                public static LocalText ClickToDefine = "*** Click to define a new one ***";
                public static LocalText NoResultsClickToDefine = "*** No results. Click to define a new one***";

                static SelectEditor()
                {
                    LocalText.InitializeTextClass(typeof(SelectEditor), "Controls.SelectEditor.");
                }
            }
        }
    }
}