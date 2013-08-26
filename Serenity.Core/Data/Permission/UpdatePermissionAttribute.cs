using System;

namespace Serenity.Data
{
    public class UpdatePermissionAttribute : OperationPermissionAttribute
    {
        public UpdatePermissionAttribute(string permission)
            : base(permission)
        {
        }
    }
}