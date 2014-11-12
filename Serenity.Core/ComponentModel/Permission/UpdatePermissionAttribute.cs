using System;

namespace Serenity.Data
{
    public class UpdatePermissionAttribute : PermissionAttributeBase
    {
        public UpdatePermissionAttribute(string permission)
            : base(permission)
        {
        }

        public UpdatePermissionAttribute(object module, string permission)
            : base(module, permission)
        {
        }

        public UpdatePermissionAttribute(object module, object submodule, string permission)
            : base(module, submodule, permission)
        {
        }
    }
}