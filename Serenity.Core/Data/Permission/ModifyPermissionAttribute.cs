using System;

namespace Serenity.Data
{
    public class ModifyPermissionAttribute : OperationPermissionAttribute
    {
        public ModifyPermissionAttribute(string permission)
            : base(permission)
        {
        }

        public ModifyPermissionAttribute(object applicationId, string permission)
            : this(applicationId.ToString() + ":" + permission)
        {
        }
    }
}