using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Reflection;

namespace Serenity.Testing
{
    public class DbOverride
    {
        private static class Cached<TDbScript>
        {
            public static string Script;
            public static string ScriptHash;
        }

        public DbOverride(string connectionKey, string dbAlias, string script)
        {
            this.ConnectionKey = connectionKey;
            this.DbAlias = dbAlias;
            this.Script = script;
            this.ScriptHash = DbManager.GetHash(script ?? "");
        }

        private DbOverride(string connectionKey, string dbAlias, string script, string scriptHash)
        {
            this.ConnectionKey = connectionKey;
            this.DbAlias = dbAlias;
            this.Script = script;
            this.ScriptHash = scriptHash;
        }

        public static DbOverride New<TDbScript>(string connectionKey = null, string dbAlias = null, bool cacheScript = true)
            where TDbScript : DbScript, new()
        {
            if (connectionKey == null)
            {
                var connectionKeyAttr = typeof(TDbScript).GetCustomAttribute<ConnectionKeyAttribute>();
                if (connectionKeyAttr == null || string.IsNullOrEmpty(connectionKeyAttr.Value))
                    throw new ArgumentNullException("connectionKey");

                connectionKey = connectionKeyAttr.Value;
            }

            if (dbAlias == null)
            {
                var dbAliasAttr = typeof(TDbScript).GetCustomAttribute<DatabaseAliasAttribute>();
                if (dbAliasAttr == null)
                {
                    dbAlias = connectionKey;
                }
                else
                {
                    dbAlias = dbAliasAttr.Value;
                }
            }

            if (cacheScript)
            {
                if (Cached<TDbScript>.Script == null)
                {
                    var script = new TDbScript().ToString();
                    Cached<TDbScript>.Script = script ?? "";
                    Cached<TDbScript>.ScriptHash = DbManager.GetHash(script ?? "");
                }

                return new DbOverride(connectionKey, dbAlias, Cached<TDbScript>.Script, Cached<TDbScript>.ScriptHash);
            }
            else
            {
                var script = new TDbScript().ToString();
                return new DbOverride(connectionKey, dbAlias, script);
            }
        }

        public string DbAlias { get; private set; }
        public string Script { get; private set; }
        public string ScriptHash { get; private set; }
        public string ConnectionKey { get; private set; }
    }
}