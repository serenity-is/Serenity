using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Serenity.Testing
{
    public class DbManager
    {
        static DbManager()
        {
            try
            {
                RemoveTestFiles();
            }
            catch (Exception)
            {
            }
        }

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

        public static void RenameDb(string oldAlias, string newAlias)
        {
            using (var connection = SqlConnections.New(DbSettings.Current.ServerConnectionString, DbSettings.Current.Provider))
            {
                SqlConnection.ClearAllPools();
                SqlHelper.ExecuteNonQuery(connection, String.Format(
                    "if db_id('{0}') is not null\n" +
                    "BEGIN\n" +
                    "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                    "ALTER DATABASE [{0}] MODIFY NAME = {1};" +
                    "ALTER DATABASE [{0}] SET MULTI_USER;" +
                    "END\n", oldAlias, newAlias));
            }
        }

        public static void DetachDb(string dbAlias)
        {
            using (var connection = SqlConnections.New(DbSettings.Current.ServerConnectionString, DbSettings.Current.Provider))
            {
                SqlConnection.ClearAllPools();
                try
                {
                    SqlHelper.ExecuteNonQuery(connection, String.Format(
                        "if db_id('{0}') is not null\n" +
                        "BEGIN\n" +
                        "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                        "EXEC sp_detach_db '{0}', 'true'\n" +
                        "END\n", dbAlias));
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode == -2146232060) // physical database file doesn't exist (manually deleted?)
                        SqlHelper.ExecuteNonQuery(connection, String.Format(
                            "DROP DATABASE [{0}]", dbAlias));
                    else
                        throw;
                }
            }
        }

        public static void AttachDb(string dbAlias, string mdfFilePath)
        {
            using (var connection = SqlConnections.New(DbSettings.Current.ServerConnectionString, DbSettings.Current.Provider))
            {
                var dbFile = Path.ChangeExtension(mdfFilePath, null);
                var sql = String.Format(
                    "CREATE DATABASE [{0}] ON (FILENAME = '{1}.mdf'), (FILENAME = '{1}.ldf') FOR ATTACH;",
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

            using (var connection = SqlConnections.New(DbSettings.Current.ServerConnectionString, DbSettings.Current.Provider))
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

            string connectionString = String.Format(DbSettings.Current.ConnectionStringFormat, dbAlias);

            if (!script.IsNullOrEmpty())
                try
                {
                    using (var connection = SqlConnections.New(connectionString, DbSettings.Current.Provider))
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

        public static void RemoveTestFiles()
        {
            HashSet<string> files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var connection = SqlConnections.New(DbSettings.Current.ServerConnectionString, DbSettings.Current.Provider))
            using (var reader = SqlHelper.ExecuteReader(connection, "SELECT physical_name FROM sys.master_files"))
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        files.Add(Path.GetFileName(reader.GetString(0)));
                }
            }

            foreach (var file in Directory.GetFiles(DbSettings.Current.RootPath, "test*.*"))
                if (!files.Contains(Path.GetFileName(file)))
                    TemporaryFileHelper.TryDelete(file);
        }

        public static string GetHash(string script)
        {
            string hash;
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(script));
                hash = BitConverter.ToString(hashBytes).Replace("-", "");
            }

            return hash;
        }

        public static string CreateDatabaseFilesForScript(string script)
        {
            var hash = GetHash(script);

            var cachedPath = Path.Combine(DbSettings.Current.RootPath,
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
            var instancePath = Path.Combine(DbSettings.Current.RootPath,
                "test_" + rnd + ".mdf");

            CopyDb(cachedPath, instancePath);

            return instancePath;
        }
    }
}