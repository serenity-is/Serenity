using System;

namespace Serenity.Data
{
    public class DeletePermissionAttribute : OperationPermissionAttribute
    {
        public DeletePermissionAttribute(string permission)
            : base(permission)
        {
        }
    }
}