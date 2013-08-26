using System;

namespace Serenity.Data
{
    public class InsertPermissionAttribute : OperationPermissionAttribute
    {
        public InsertPermissionAttribute(string permission)
            : base(permission)
        {
        }
    }
}