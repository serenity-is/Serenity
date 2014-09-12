namespace BasicApplication
{
    using Serenity;
    using Serenity.Data;
    using System;

    public static partial class SiteInitialization
    {
        public static void ApplicationStart()
        {
            try
            {
                SqlSettings.CurrentDialect = SqlDialect.MsSql2012;

                Serenity.Web.CommonInitialization.Run();

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                //registrar.RegisterInstance<IUserRetrieveService>(new MasterDB.Repositories.KullaniciRepository.UserRetrieveService());
                //registrar.RegisterInstance<IAuthenticationService>(new MasterDB.Repositories.KullaniciRepository.AuthenticationService());
                //registrar.RegisterInstance<IPermissionService>(new MasterDB.Repositories.KullaniciRepository.PermissionService());
                //registrar.RegisterInstance<IAuthorizationService>(new MasterDB.Repositories.KullaniciRepository.AuthorizationService());
            }
            catch (Exception ex)
            {
                ex.Log();
                throw;
            }
        }

        public static void ApplicationEnd()
        {
            Log.Dispose();
        }
    }
}