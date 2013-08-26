using System;

namespace Serenity.Data
{
    public class ModifyPermissionAttribute : OperationPermissionAttribute
    {
        public ModifyPermissionAttribute(string permission)
            : base(permission)
        {
        }
    }
}