using System;

namespace Serenity.Data
{
    public class DeletePermissionAttribute : PermissionAttributeBase
    {
        public DeletePermissionAttribute(object permission)
            : base(permission)
        {
        }

        public DeletePermissionAttribute(object module, object permission)
            : base(module, permission)
        {
        }

        public DeletePermissionAttribute(object module, object submodule, object permission)
            : base(module, submodule, permission)
        {
        }
    }
}