using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity.Testing
{
    public class DbTestContext : IDisposable
    {
        private bool disposed;
        private List<Tuple<string, string>> overrides;
        
        private static class Cached<TDbScript>
        {
            public static string Script;
        }

        public DbTestContext()
        {
            overrides = new List<Tuple<string, string>>();
        }

        public void Override(string connectionKey, string dbAlias, string script)
        {
            DbManager.DetachDb(dbAlias);
            var mdfFilePath = DbManager.CreateDatabaseFilesForScript(script);
            overrides.Add(new Tuple<string, string>(dbAlias, mdfFilePath));
            DbManager.AttachDb(dbAlias, mdfFilePath);
            SqlConnections.SetConnection(connectionKey,
                String.Format(DbSettings.ConnectionStringFormat, dbAlias), DbSettings.ProviderName);
        }

        public void Override<TDbScript>(string connectionKey, string dbAlias, bool cached = true)
            where TDbScript: DbScript, new()
        {
            string script;
            if (cached)
                script = Cached<TDbScript>.Script = Cached<TDbScript>.Script ?? new TDbScript().ToString();
            else
                script = new TDbScript().ToString();

            Override(connectionKey, dbAlias, script);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                foreach (var db in overrides)
                {
                    DbManager.DetachDb(db.Item1);
                    DbManager.DeleteDb(db.Item2);
                }

                disposed = true;
            }
        }

        ~DbTestContext()
        {
            try
            {
                Dispose();
            }
            catch
            {
            }
        }    
    }
}