using System;

namespace Serenity.Data
{
    public class UpdatePermissionAttribute : PermissionAttributeBase
    {
        public UpdatePermissionAttribute(object permission)
            : base(permission)
        {
        }

        public UpdatePermissionAttribute(object module, object permission)
            : base(module, permission)
        {
        }

        public UpdatePermissionAttribute(object module, object submodule, object permission)
            : base(module, submodule, permission)
        {
        }
    }
}