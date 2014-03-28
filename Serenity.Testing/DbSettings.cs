using Newtonsoft.Json;
using System.Configuration;

namespace Serenity.Testing
{
    public class DbSettings
    {
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
            get { return "Data Source=" + Server + ";Integrated Security=true;"; }
        }

        public string ConnectionStringFormat
        {
            get { return "Data Source=" + Server + ";Initial Catalog={0};Integrated Security=true;"; }
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