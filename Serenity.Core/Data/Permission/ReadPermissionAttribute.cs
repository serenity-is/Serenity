using System;

namespace Serenity.Data
{
    public class ReadPermissionAttribute : OperationPermissionAttribute
    {
        public ReadPermissionAttribute(string permission)
            : base(permission)
        {
        }
    }
}