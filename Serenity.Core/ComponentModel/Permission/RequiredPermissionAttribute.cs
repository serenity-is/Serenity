using Serenity.Data;
using System;

namespace Serenity.ComponentModel
{
    public class RequiredPermissionAttribute : PermissionAttributeBase
    {
        public RequiredPermissionAttribute(string permission)
            : base(permission)
        {
        }

        public RequiredPermissionAttribute(object module, string permission)
            : base(module, permission)
        {
        }

        public RequiredPermissionAttribute(object module, object submodule, string permission)
            : base(module, submodule, permission)
        {
        }
    }
}