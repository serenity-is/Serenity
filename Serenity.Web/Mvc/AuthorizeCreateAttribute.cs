using Serenity.Data;
using System;

namespace Serenity.Services
{
    public class AuthorizeCreateAttribute : ServiceAuthorizeAttribute
    {
        public AuthorizeCreateAttribute(Type sourceType)
            : base(sourceType, typeof(InsertPermissionAttribute), 
                  typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
        {
        }
    }
}