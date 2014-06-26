using System;

namespace Serenity.Data
{
    public class DeletePermissionAttribute : OperationPermissionAttribute
    {
        public DeletePermissionAttribute(string permission)
            : base(permission)
        {
        }

        public DeletePermissionAttribute(object applicationId, string permission)
            : this(applicationId.ToString() + ":" + permission)
        {
        }
    }
}