using System;

namespace Serenity.Data
{
    public class ModifyPermissionAttribute : PermissionAttributeBase
    {
        public ModifyPermissionAttribute(string permission)
            : base(permission)
        {
        }

        public ModifyPermissionAttribute(object module, string permission)
            : base(module, permission)
        {
        }

        public ModifyPermissionAttribute(object module, object submodule, string permission)
            : base(module, submodule, permission)
        {
        }
    }
}