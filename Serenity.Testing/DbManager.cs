using Serenity.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Serenity.Testing
{
    public class DbManager
    {
        public static void CopyDb(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            File.Copy(sourcePath, targetPath);
            File.Copy(Path.ChangeExtension(sourcePath, ".ldf"), Path.ChangeExtension(targetPath, ".ldf"));
        }
        
        public static void DeleteDb(string path)
        {
            TemporaryFileHelper.TryDelete(path);
            TemporaryFileHelper.TryDelete(Path.ChangeExtension(path, ".ldf"));
        }

        public static void DetachDb(string dbAlias)
        {
            using (var connection = SqlConnections.New(DbSettings.LocalDbConnectionString, DbSettings.ProviderName))
            {
                SqlConnection.ClearAllPools();
                SqlHelper.ExecuteNonQuery(connection, String.Format(
                    "if db_id('{0}') is not null\n" +
                    "BEGIN\n" +
                    "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                    "EXEC sp_detach_db '{0}', 'true'\n" +
                    "END\n", dbAlias));
            }
        }

        public static void AttachDb(string dbAlias, string mdfFilePath)
        {
            using (var connection = SqlConnections.New(DbSettings.LocalDbConnectionString, DbSettings.ProviderName))
            {
                var dbFile = Path.ChangeExtension(mdfFilePath, null);
                var sql = String.Format(
                    "CREATE DATABASE {0} ON (FILENAME = '{1}.mdf'), (FILENAME = '{1}.ldf') FOR ATTACH;",
                    dbAlias, dbFile);

                SqlHelper.ExecuteNonQuery(connection, sql);
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

        public static void CreateDb(string dbAlias, string mdfFilePath, string script)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(mdfFilePath));

            using (var connection = SqlConnections.New(DbSettings.LocalDbConnectionString, DbSettings.ProviderName))
            {
                string createScript;

                using (var stream = typeof(DbManager).Assembly
                    .GetManifestResourceStream("Serenity.Testing.SqlScript.CreateDatabase.sql"))
                {
                    using (var sr = new StreamReader(stream))
                    {
                        createScript = sr.ReadToEnd();
                    }
                }

                createScript = createScript.Replace("$$DbAlias$$", dbAlias).Replace("$$DbPath$$", 
                    Path.ChangeExtension(mdfFilePath, null));

                RunScript(connection, createScript);
            }

            string connectionString = String.Format(DbSettings.ConnectionStringFormat, dbAlias);

            if (!script.IsEmptyOrNull())
                try
                {
                    using (var connection = SqlConnections.New(connectionString, DbSettings.ProviderName))
                    {
                        RunScript(connection, script);
                    }
                }
                catch 
                {
                    DetachDb(dbAlias);
                    DeleteDb(mdfFilePath);
                    throw;
                }
        }

        public static string CreateDatabaseFilesForScript(string script)
        {
            string hash;
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(script));
                hash = BitConverter.ToString(hashBytes).Replace("-", "");
            }

            var cachedPath = Path.Combine(DbSettings.TestRootPath,
                "cache_" + hash + ".mdf");

            if (!File.Exists(cachedPath))
            {
                try
                {
                    CreateDb("cache_" + hash, cachedPath, script);
                }
                finally
                {
                    DetachDb("cache_" + hash);
                }
            }
            else
                DetachDb("cache_" + hash);

            if (!File.Exists(cachedPath))
                throw new InvalidOperationException("Test için cache veritabanı oluşturulamadı!");

            var rnd = TemporaryFileHelper.RandomFileCode();
            var instancePath = Path.Combine(DbSettings.TestRootPath,
                "test_" + rnd + ".mdf");

            CopyDb(cachedPath, instancePath);

            return instancePath;
        }
    }
}