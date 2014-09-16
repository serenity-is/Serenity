namespace BasicApplication
{
    using Serenity;
    using Serenity.Abstractions;
    using Serenity.Data;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Web.Hosting;

    public static partial class SiteInitialization
    {
        public static void ApplicationStart()
        {
            try
            {
                SqlSettings.CurrentDialect = SqlDialect.MsSql2012;

                Serenity.Web.CommonInitialization.Run();

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<IAuthorizationService>(new Administration.AuthorizationService());
                registrar.RegisterInstance<IAuthenticationService>(new Administration.AuthenticationService());
                registrar.RegisterInstance<IPermissionService>(new Administration.PermissionService());
                registrar.RegisterInstance<IUserRetrieveService>(new Administration.UserRetrieveService());
            }
            catch (Exception ex)
            {
                ex.Log();
                throw;
            }

            EnsureDatabase();
        }

        public static void ApplicationEnd()
        {
            Log.Dispose();
        }

        private static void EnsureDatabase()
        {
            using (var connection = SqlConnections.NewByKey("Default"))
            try
            {
                connection.Open();
            }
            catch
            {
                var cb = new DbConnectionStringBuilder();
                cb.ConnectionString = SqlConnections.GetConnectionString("Default").Item1;
                var catalog = cb["Initial Catalog"];
                cb["Initial Catalog"] = null;
                cb["AttachDBFilename"] = null;

                using (var serverConnection = new SqlConnection(cb.ConnectionString))
                {
                    serverConnection.Open();
                    serverConnection.Execute(String.Format(
                        @"CREATE DATABASE [{0}] ON PRIMARY (Name = N'{0}', FILENAME = '{1}\{0}.mdf') LOG ON (NAME = N'{0}_log', FILENAME = '{1}\{0}.ldf');",
                            catalog, HostingEnvironment.MapPath("~/App_Data")));
                }
                
                SqlConnection.ClearAllPools();
            }

            EnsureDBScript();
        }

        private static void EnsureDBScript()
        {
            using (var connection = SqlConnections.NewByKey("Default"))
            {
                connection.Open();

                var tables = ((DbConnection)(((WrappedConnection)connection).ActualConnection)).GetSchema("Tables");
                foreach (DataRow row in tables.Rows)
                {
                    if (String.Compare(row["TABLE_NAME"] as string, "Users", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return;
                    }
                }

                RunScript(connection, File.ReadAllText(HostingEnvironment.MapPath("~/App_Start/BasicDBScript.sql")));
                RunScript(connection, File.ReadAllText(HostingEnvironment.MapPath("~/App_Start/NorthwindDBScript.sql")));
            }
        }

        public static void RunScript(IDbConnection connection, string script)
        {
            var parts = script.Replace("\r", "").Split(new string[] { "GO\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                try
                {
                    SqlHelper.ExecuteNonQuery(connection, part);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Error executing script: {1}\n\n{0}",
                        part, ex.ToString()), ex);
                }
            }
        }
    }
}