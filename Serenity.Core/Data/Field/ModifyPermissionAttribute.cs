using System;

namespace Serenity.Data
{
    public class ModifyPermissionAttribute : Attribute
    {
        public ModifyPermissionAttribute(string modifyPermission)
        {
            this.ModifyPermission = modifyPermission;
        }

        public string ModifyPermission { get; private set; }
    }
}