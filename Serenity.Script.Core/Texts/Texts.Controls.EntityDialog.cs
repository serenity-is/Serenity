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
            public static class EntityDialog
            {
                public static LocalText DeleteConfirmation = "Delete record?";
                public static LocalText UndeleteButton = "Undelete";
                public static LocalText UndeleteConfirmation = "Undelete record?";
                public static LocalText CloneButton = "Clone";
                public static LocalText SaveSuccessMessage = "Save success";
                public static LocalText SaveButton = "Save";
                public static LocalText UpdateButton = "Update";
                public static LocalText ApplyChangesButton = "Apply Changes";
                public static LocalText DeleteButton = "Delete";
                public static LocalText LocalizationButton = "Localization";
                public static LocalText LocalizationConfirmation = "Save changes to translations?";
                public static LocalText NewRecordTitle = "New {0}";
                public static LocalText EditRecordTitle = "Edit {0}{1}";

                static EntityDialog()
                {
                    LocalText.InitializeTextClass(typeof(EntityDialog), "Controls.EntityDialog.");
                }
            }
        }
    }
}