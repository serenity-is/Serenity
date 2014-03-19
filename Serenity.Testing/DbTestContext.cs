using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Serenity.Testing
{
    public class DbOverride
    {
        private static class Cached<TDbScript>
        {
            public static string Script;
        }

        public DbOverride(string connectionKey, string dbAlias, string script)
        {
            this.ConnectionKey = connectionKey;
            this.DbAlias = dbAlias;
            this.Script = script;
            this.ScriptHash = DbManager.GetHash(script ?? "");
        }

        public static DbOverride New<TDbScript>(string connectionKey, string dbAlias, bool cached = true)
            where TDbScript : DbScript, new()
        {
            string script;
            if (cached)
                script = Cached<TDbScript>.Script = Cached<TDbScript>.Script ?? new TDbScript().ToString();
            else
                script = new TDbScript().ToString();

            return new DbOverride(connectionKey, dbAlias, script);
        }

        public string DbAlias { get; private set; }
        public string Script { get; private set; }
        public string ScriptHash { get; private set; }
        public string ConnectionKey { get; private set; }
    }

    public class DbTestContext : IDisposable
    {
        private static object syncLock = new object();
        private static Dictionary<string, string> attachedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private bool disposed;
        private DbOverride[] overrides;
        private TransactionScope scope;
        
        public DbTestContext(params DbOverride[] overrides)
        {
            this.overrides = overrides;
            SetupOverrides();
            this.scope = new TransactionScope();
        }

        private void SetupOverrides()
        {
            foreach (var over in overrides)
                SetupOverride(over);
        }

        private void SetupOverride(DbOverride over)
        {
            lock (syncLock)
            {
                if (attachedHashes.ContainsKey(over.DbAlias))
                {
                    var currentHash = attachedHashes[over.DbAlias];

                    if (currentHash == over.ScriptHash)
                        return;

                    var newAlias = over.DbAlias + "_" + currentHash;
                    DbManager.DetachDb(newAlias);
                    attachedHashes.Remove(newAlias);
                    DbManager.RenameDb(over.DbAlias, newAlias);
                    attachedHashes[newAlias] = attachedHashes[over.DbAlias];
                    attachedHashes.Remove(over.DbAlias);
                }

                string possibleOldAlias = over.DbAlias + "_" + over.ScriptHash;
                if (attachedHashes.ContainsKey(possibleOldAlias))
                {
                    DbManager.DetachDb(over.DbAlias);
                    attachedHashes.Remove(over.DbAlias);
                    DbManager.RenameDb(possibleOldAlias, over.DbAlias);
                    attachedHashes.Remove(possibleOldAlias);
                    attachedHashes[over.DbAlias] = over.ScriptHash;
                    return;
                }

                attachedHashes.Remove(over.DbAlias);
                DbManager.DetachDb(over.DbAlias);
                var mdfFilePath = DbManager.CreateDatabaseFilesForScript(over.Script);
                try
                {
                    DbManager.AttachDb(over.DbAlias, mdfFilePath);
                }
                catch (Exception)
                {
                    DbManager.DetachDb(over.DbAlias);
                    DbManager.DeleteDb(mdfFilePath);
                    throw;
                }

                attachedHashes[over.DbAlias] = over.ScriptHash;

                SqlConnections.SetConnection(over.ConnectionKey,
                    String.Format(DbSettings.Current.ConnectionStringFormat, over.DbAlias), DbSettings.Current.Provider);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (scope != null)
                    try
                    {
                        scope.Dispose();
                    }
                    catch
                    {
                        // thanks to nhibernate
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