using System;

namespace Serenity.Data
{
    public class InsertPermissionAttribute : PermissionAttributeBase
    {
        public InsertPermissionAttribute(object permission)
            : base(permission)
        {
        }

        public InsertPermissionAttribute(object module, object permission)
            : base(module, permission)
        {
        }

        public InsertPermissionAttribute(object module, object submodule, object permission)
            : base(module, submodule, permission)
        {
        }
    }
}