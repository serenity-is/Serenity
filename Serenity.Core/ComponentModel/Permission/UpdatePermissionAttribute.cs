using System;

namespace Serenity.Data
{
    public class UpdatePermissionAttribute : OperationPermissionAttribute
    {
        public UpdatePermissionAttribute(string permission)
            : base(permission)
        {
        }

        public UpdatePermissionAttribute(object applicationId, string permission)
            : this(applicationId.ToString() + ":" + permission)
        {
        }
    }
}