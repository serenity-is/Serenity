using System;

namespace Serenity.Data
{
    public class ReadPermissionAttribute : PermissionAttributeBase
    {
        public ReadPermissionAttribute(string permission)
            : base(permission)
        {
        }

        public ReadPermissionAttribute(object module, string permission)
            : base(module, permission)
        {
        }

        public ReadPermissionAttribute(object module, object submodule, string permission)
            : base(module, submodule, permission)
        {
        }
    }
}