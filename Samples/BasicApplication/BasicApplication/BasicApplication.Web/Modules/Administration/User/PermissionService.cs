namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.Abstractions;

    public class PermissionService : IPermissionService
    {
        public bool HasPermission(string permission)
        {
            if (Authorization.Username == "admin")
                return true;

            if (permission == "Administration")
                return false;

            return true;
        }
    }
}