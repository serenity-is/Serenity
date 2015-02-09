using System;

namespace Serenity.Data
{
    public class ModifyPermissionAttribute : PermissionAttributeBase
    {
        public ModifyPermissionAttribute(object permission)
            : base(permission)
        {
        }

        public ModifyPermissionAttribute(object module, object permission)
            : base(module, permission)
        {
        }

        public ModifyPermissionAttribute(object module, object submodule, object permission)
            : base(module, submodule, permission)
        {
        }
    }
}