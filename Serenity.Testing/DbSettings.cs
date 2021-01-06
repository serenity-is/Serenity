using Newtonsoft.Json;
using System;
using System.Configuration;

namespace Serenity.Testing
{
    public class DbSettings
    {
        private bool initializedLocalDb;

        public DbSettings()
        {
            Server = @"(localdb)\test";
            Provider = "System.Data.SqlClient";
            RootPath = @"c:\.localdbtest";
        }

        public string Server { get; set; }
        public string Provider { get; set; }
        public string RootPath { get; set; }

        public string ServerConnectionString
        {
            get
            {
                if (!initializedLocalDb &&
                    Server.StartsWith("(localdb)\\", System.StringComparison.OrdinalIgnoreCase))
                {
                    initializedLocalDb = true;

                    try
                    {
                        var instanceName = Server.Substring("(localdb)\\".Length);

                        if (!System.Data.SqlLocalDb.SqlLocalDbApi.GetInstanceNames().Contains(instanceName))
                            System.Data.SqlLocalDb.SqlLocalDbApi.CreateInstance(instanceName);

                        System.Data.SqlLocalDb.SqlLocalDbApi.StartInstance(instanceName);
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                    }
                }

                return "Data Source=" + Server + ";Integrated Security=true;Connection Timeout=180";
            }
        }

        public string ConnectionStringFormat
        {
            get { return "Data Source=" + Server + ";Initial Catalog={0};Integrated Security=true;Connection Timeout=180"; }
        }

        private static DbSettings current;

        public static DbSettings Current
        {
            get
            {
                if (current == null)
                    current = JsonConvert.DeserializeObject<DbSettings>(
                        ConfigurationManager.AppSettings["TestDbSettings"].TrimToNull() ?? "{}");

                return current;
            }
        }
    }
}