using System;

namespace Serenity.Data
{
    public class DeletePermissionAttribute : PermissionAttributeBase
    {
        public DeletePermissionAttribute(string permission)
            : base(permission)
        {
        }

        public DeletePermissionAttribute(object module, string permission)
            : base(module, permission)
        {
        }

        public DeletePermissionAttribute(object module, object submodule, string permission)
            : base(module, submodule, permission)
        {
        }
    }
}