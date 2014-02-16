using System;

namespace Serenity.Reporting
{
    public class RequiredPermissionAttribute : Attribute
    {
        public RequiredPermissionAttribute(string permission)
        {
            this.Permission = permission;
        }

        public string Permission { get; private set; }
    }
}