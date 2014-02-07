namespace Serenity.Testing
{
    public static class DbSettings
    {
        private readonly static string LocalDbServerName = @"(localdb)\test";
        public readonly static string ProviderName = "System.Data.SqlClient";
        public readonly static string TestRootPath = @"c:\.localdbtest";
        public readonly static string LocalDbConnectionString = 
            @"Data Source=" + LocalDbServerName + ";Integrated Security=true;";
        public readonly static string ConnectionStringFormat = 
            @"Data Source=" + LocalDbServerName + ";Initial Catalog={0};Integrated Security=true;";

        //public readonly static string SessionGuid = "s" + DataHelper.RandomFileCode();
        //public readonly static string ServerName = @".\SqlExpress";
        //public readonly static string ServerUser = @"sinerji_iusr";
        //public readonly static string ServerPass = @"sinerji_iusr_308206";
        //public readonly static string ServerConnection = String.Format(@"Data Source={0}; Integrated Security=true;", ServerName);
        //public readonly static string DatabaseConnection = @"Data Source=" + ServerName +
        //    ";Initial Catalog={0};User ID=" + ServerUser + ";password=" + ServerPass + ";Integrated Security=false;";
        //public readonly static string DatabasePath = @"C:\TestDb\" + SessionGuid + @"\";
    }
}