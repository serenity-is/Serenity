using Serenity.Data;
using System;

namespace Serenity.ComponentModel
{
    public class RequiredPermissionAttribute : PermissionAttributeBase
    {
        public RequiredPermissionAttribute(object permission)
            : base(permission)
        {
        }

        public RequiredPermissionAttribute(object module, object permission)
            : base(module, permission)
        {
        }

        public RequiredPermissionAttribute(object module, object submodule, object permission)
            : base(module, submodule, permission)
        {
        }
    }
}