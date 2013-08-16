using System;

namespace Serenity.Data
{
    public class ReadPermissionAttribute : Attribute
    {
        public ReadPermissionAttribute(string readPermission)
        {
            this.ReadPermission = readPermission;
        }

        public string ReadPermission { get; private set; }
    }
}