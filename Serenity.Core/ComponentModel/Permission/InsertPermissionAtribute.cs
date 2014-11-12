using System;

namespace Serenity.Data
{
    public class InsertPermissionAttribute : PermissionAttributeBase
    {
        public InsertPermissionAttribute(string permission)
            : base(permission)
        {
        }

        public InsertPermissionAttribute(object module, string permission)
            : base(module, permission)
        {
        }

        public InsertPermissionAttribute(object module, object submodule, string permission)
            : base(module, submodule, permission)
        {
        }
    }
}