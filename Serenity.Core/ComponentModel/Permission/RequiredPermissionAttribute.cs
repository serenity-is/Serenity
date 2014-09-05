using System;

namespace Serenity.ComponentModel
{
    public class RequiredPermissionAttribute : Attribute
    {
        public RequiredPermissionAttribute(string permission)
        {
            this.Permission = permission;
        }

        public RequiredPermissionAttribute(object applicationId, string permission)
            : this(applicationId.ToString() + ":" + permission)
        {
        }

        public string Permission { get; private set; }
    }
}