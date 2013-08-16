using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [ScriptName("Texts"), IgnoreNamespace, PreserveMemberCase]
    public static partial class Texts
    {
        static Texts()
        {
            LocalText.InitializeTextClass(typeof(Dialogs), "Dialogs.");
        }
    }
}