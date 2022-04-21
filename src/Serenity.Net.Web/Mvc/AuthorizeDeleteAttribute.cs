namespace Serenity.Services
{
    public class AuthorizeDeleteAttribute : ServiceAuthorizeAttribute
    {
        public AuthorizeDeleteAttribute(Type sourceType)
            : base(sourceType, typeof(DeletePermissionAttribute),
                  typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
        {
        }
    }
}