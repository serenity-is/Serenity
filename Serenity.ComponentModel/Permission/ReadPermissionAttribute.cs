using System;

namespace Serenity.Data
{
    public class ReadPermissionAttribute : OperationPermissionAttribute
    {
        public ReadPermissionAttribute(string permission)
            : base(permission)
        {
        }

        public ReadPermissionAttribute(object applicationId, string permission)
            : this(applicationId.ToString() + ":" + permission)
        {
        }
    }
}