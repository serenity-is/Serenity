using Serenity.Data;
using System;

namespace Serenity.Services
{
    public class AuthorizeUpdateAttribute : ServiceAuthorizeAttribute
    {
        public AuthorizeUpdateAttribute(Type sourceType)
            : base(sourceType, typeof(UpdatePermissionAttribute),
                  typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
        {
        }
    }
}