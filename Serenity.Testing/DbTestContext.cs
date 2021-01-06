using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Reflection;

namespace Serenity.Testing
{
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

        public void ExpireOverrides()
        {
            lock (syncLock)
            {
                foreach (var over in overrides)
                    attachedHashes.Remove(over.DbAlias);

                if (scope != null)
                {
                    scope.Dispose();
                    scope = null;
                }
            }
        }

        private void SetupOverride(DbOverride over)
        {
            lock (syncLock)
            {
                Action setConnectionString = delegate
                {
                    SqlConnections.SetConnection(over.ConnectionKey,
                        String.Format(DbSettings.Current.ConnectionStringFormat, over.DbAlias), DbSettings.Current.Provider);
                };

                if (attachedHashes.ContainsKey(over.DbAlias))
                {
                    var currentHash = attachedHashes[over.DbAlias];

                    if (currentHash == over.ScriptHash)
                    {
                        setConnectionString();
                        return;
                    }

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
                    setConnectionString();
                    return;
                }

                attachedHashes.Remove(over.DbAlias);
                DbManager.DetachDb(over.DbAlias);
                var mdfFilePath = DbManager.CreateDatabaseFilesForScript(over.Script, over.ScriptHash);
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
                setConnectionString();
            }
        }

        private void FreeOverrides()
        {
            if (overrides != null)
                foreach (var over in overrides)
                {
                    SqlConnections.SetConnection(over.ConnectionKey, "!!!InvalidConnectionString!!!", DbSettings.Current.Provider);
                }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                FreeOverrides();

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

    public class DbTestContext<TDbScript> : DbTestContext
        where TDbScript : DbScript, new()
    {
        public DbTestContext()
            : base(DbOverride.New<TDbScript>())
        {
        }
    }

    public class DbTestContext<TDbScript1, TDbScript2> : DbTestContext
        where TDbScript1 : DbScript, new()
        where TDbScript2 : DbScript, new()
    {
        public DbTestContext()
            : base(
                DbOverride.New<TDbScript1>(),
                DbOverride.New<TDbScript2>())
        {
        }
    }

    public class DbTestContext<TDbScript1, TDbScript2, TDbScript3> : DbTestContext
        where TDbScript1 : DbScript, new()
        where TDbScript2 : DbScript, new()
        where TDbScript3 : DbScript, new()
    {
        public DbTestContext()
            : base(
                DbOverride.New<TDbScript1>(),
                DbOverride.New<TDbScript2>(),
                DbOverride.New<TDbScript3>())
        {
        }
    }

    public class DbTestContext<TDbScript1, TDbScript2, TDbScript3, TDbScript4> : DbTestContext
        where TDbScript1 : DbScript, new()
        where TDbScript2 : DbScript, new()
        where TDbScript3 : DbScript, new()
        where TDbScript4 : DbScript, new()
    {
        public DbTestContext()
            : base(
                DbOverride.New<TDbScript1>(),
                DbOverride.New<TDbScript2>(),
                DbOverride.New<TDbScript3>(),
                DbOverride.New<TDbScript4>())
        {
        }
    }

    public class DbTestContext<TDbScript1, TDbScript2, TDbScript3, TDbScript4, TDbScript5> : DbTestContext
        where TDbScript1 : DbScript, new()
        where TDbScript2 : DbScript, new()
        where TDbScript3 : DbScript, new()
        where TDbScript4 : DbScript, new()
        where TDbScript5 : DbScript, new()
    {
        public DbTestContext()
            : base(
                DbOverride.New<TDbScript1>(),
                DbOverride.New<TDbScript2>(),
                DbOverride.New<TDbScript3>(),
                DbOverride.New<TDbScript4>(),
                DbOverride.New<TDbScript5>())
        {
        }
    }
}