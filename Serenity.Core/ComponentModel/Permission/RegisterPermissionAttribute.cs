using Serenity.Data;
using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterPermissionKeyAttribute : PermissionAttributeBase
    {
        public RegisterPermissionKeyAttribute(object permission)
            : base(permission)
        {
        }

        public RegisterPermissionKeyAttribute(object module, object permission)
            : base(module, permission)
        {
        }

        public RegisterPermissionKeyAttribute(object module, object submodule, object permission)
            : base(module, submodule, permission)
        {
        }
    }
}