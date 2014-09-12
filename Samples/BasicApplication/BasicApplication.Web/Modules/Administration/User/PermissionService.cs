namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.Abstractions;

    public class PermissionService : IPermissionService
    {
        public bool HasPermission(string permission)
        {
            if (permission == "Administration")
                return Authorization.AdminUsername != null && Authorization.AdminUsername == Authorization.Username;

            return true;
        }
    }
}