using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Table;
using System.Data.Common;
using System.IO;

namespace Serenity.Extensions;

public static class MigrationUtils
{
    /// <summary>
    /// Please prefer IdentityKey(this) on the fluent column builder
    /// </summary>
    public static void CreateTableWithId32(
        this MigrationBase migration, string table, string idField,
        Action<ICreateTableColumnOptionOrWithColumnSyntax> addColumns, string schema = null, bool checkExists = false, bool primaryKey = true)
    {
        CreateTableWithId(migration, table, idField, addColumns, schema, 32, checkExists, primaryKey);
    }

    /// <summary>
    /// Please prefer IdentityKey(this) on the fluent column builder
    /// </summary>
    public static void CreateTableWithId64(
        this MigrationBase migration, string table, string idField,
        Action<ICreateTableColumnOptionOrWithColumnSyntax> addColumns, string schema = null, bool checkExists = false, bool primaryKey = true)
    {
        CreateTableWithId(migration, table, idField, addColumns, schema, 64, checkExists, primaryKey);
    }

    private static void CreateTableWithId(
        MigrationBase migration, string table, string idField,
        Action<ICreateTableColumnOptionOrWithColumnSyntax> addColumns, string schema, int size, bool checkExists = false, bool primaryKey = true)
    {
        if (checkExists && (
            (schema != null && migration.Schema.Schema(schema).Table(table).Exists()) ||
            (schema == null && migration.Schema.Table(table).Exists())))
            return;

        var createTable = migration.Create.Table(table);
        var withSchema = schema != null ? createTable.InSchema(schema) : createTable;
        var withColumn = withSchema.WithColumn(idField);
        var withType = (size switch
        {
            64 => withColumn.AsInt64(),
            16 => withColumn.AsInt16(),
            _ => withColumn.AsInt32()
        });
        var withIdentity = primaryKey ? withType.IdentityKey(migration)
            : withType.AutoIncrement(migration);
        addColumns(withIdentity);
    }

    /// <summary>
    /// Declares column as Identity() if the database is something other than Oracle,
    /// defines an Oracle sequence otherwise. It sets the column as PrimaryKey() and
    /// also calls NotNullable() as it is not possible for identity / sequence columns 
    /// to be nullable. 
    /// <param name="syntax">The WithColumn syntax builder</param>
    /// <param name="migration">The migration reference to determine the database type</param>
    public static ICreateTableColumnOptionOrWithColumnSyntax IdentityKey(this ICreateTableColumnOptionOrWithColumnSyntax syntax,
        MigrationBase migration)
    {
        syntax = syntax.NotNullable().PrimaryKey();

        if (migration.IsOracle())
        {
            var builder = ((IColumnExpressionBuilder)syntax);
            AddOracleIdentity(migration, builder.TableName, builder.Column.Name);
            return syntax;
        }

        syntax = syntax.Identity();

        return syntax;
    }


    /// <summary>
    /// Declares column as auto increment (e.g. Identity()) if the database is something other than Oracle,
    /// defines an Oracle sequence otherwise. It also calls NotNullable() as it is 
    /// not possible for auto increment / sequence columns to be nullable. This assumes the 
    /// column will NOT be set as PrimaryKey(), just as an auto incrementing value.
    /// As MySql does not support AUTO_INCREMENT without primary key or an index, this
    /// first creates the column as a regular one, then creates an index and modifies it
    /// to be an AUTO_INCREMENT.
    /// <param name="syntax">The WithColumn syntax builder</param>
    /// <param name="migration">The migration reference to determine the database type</param>
    public static ICreateTableColumnOptionOrWithColumnSyntax AutoIncrement(this ICreateTableColumnOptionOrWithColumnSyntax syntax,
        MigrationBase migration)
    {
        syntax = syntax.NotNullable();

        if (migration.IsOracle())
        {
            var builder = ((IColumnExpressionBuilder)syntax);
            AddOracleIdentity(migration, builder.TableName, builder.Column.Name);
            return syntax;
        }

        if (migration.IsMySql())
        {
            var builder = ((IColumnExpressionBuilder)syntax);
            var createIndex = migration.Create
                .Index($"IX_{builder.TableName}_{builder.Column.Name}")
                .OnTable(builder.TableName);
            if (!string.IsNullOrEmpty(builder.SchemaName))
            {
                createIndex.InSchema(builder.SchemaName)
                    .OnColumn(builder.Column.Name)
                    .Unique();
            }
            else
            {
                createIndex
                    .OnColumn(builder.Column.Name)
                    .Unique();
            }
            var type = builder.Column.Type.Value == System.Data.DbType.Int64 ||
                builder.Column.Type.Value == System.Data.DbType.UInt64 ?
                "BIGINT" : "INT";

            migration.IfDatabase("MySql").Execute.Sql(
                $"ALTER TABLE `{(string.IsNullOrEmpty(builder.SchemaName) ? "" :
                (builder.SchemaName + "."))}{builder.TableName}` MODIFY COLUMN `{builder.Column.Name}` {type} NOT NULL UNIQUE AUTO_INCREMENT;");

            return syntax;
        }

        syntax = syntax.Identity();

        return syntax;
    }

    public static void AddOracleIdentity(this MigrationBase migration,
        string table, string id)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(migration);

        var seq = table.Replace(" ", "_", StringComparison.Ordinal)
            .Replace("\"", "", StringComparison.Ordinal);
        seq = seq[..Math.Min(20, seq.Length)];
        seq += "_SEQ";

        migration.IfDatabase("Oracle")
            .Execute.Sql("CREATE SEQUENCE " + seq);

        migration.IfDatabase("Oracle")
            .Execute.Sql(string.Format(CultureInfo.InvariantCulture, @"
CREATE OR REPLACE TRIGGER {2}_TRG
BEFORE INSERT ON {0}
FOR EACH ROW
BEGIN
	IF :new.{1} IS NULL THEN
		SELECT {2}.nextval INTO :new.{1} FROM DUAL;
	END IF;
END;", table, id, seq));

        migration.IfDatabase("Oracle")
            .Execute.Sql(@"ALTER TRIGGER " + seq + "_TRG ENABLE");
    }
    
    public static TSyntax If<TSyntax>(this TSyntax syntax, bool predicate, Func<TSyntax, TSyntax> callback)
        where TSyntax : FluentMigrator.Infrastructure.IFluentSyntax
    {
        if (predicate)
            return callback(syntax);

        return syntax;
    }

    public static bool IsDatabase(this MigrationBase migration, string type)
    {
        return migration.IsDatabase(x => x.StartsWith(type, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsDatabase(this MigrationBase migration, Predicate<string> predicate)
    {
        bool isMatch = false;

        bool myPredicate(string dbType)
        {
            return (isMatch |= predicate(dbType));
        }

        migration.IfDatabase(myPredicate);
        return isMatch;
    }

    public static bool IsFirebird(this MigrationBase migration)
    {
        return IsDatabase(migration, "Firebird");
    }

    public static bool IsOracle(this MigrationBase migration)
    {
        return IsDatabase(migration, "Oracle");
    }

    public static bool IsMySql(this MigrationBase migration)
    {
        return IsDatabase(migration, "MySql");
    }

    public static bool IsPostgres(this MigrationBase migration)
    {
        return IsDatabase(migration, "Postgres");
    }

    public static bool IsSqlite(this MigrationBase migration)
    {
        return IsDatabase(migration, "Sqlite");
    }
    public static bool IsSqlServer(this MigrationBase migration)
    {
        return IsDatabase(migration, "SqlServer");
    }

    public static void EnsureDatabase(string databaseKey, string contentRoot, ISqlConnections sqlConnections)
    {
        var cs = sqlConnections.TryGetConnectionString(databaseKey)
            ?? throw new ArgumentNullException(nameof(databaseKey));
        var serverType = cs.Dialect.ServerType;
        bool isSql = serverType.StartsWith("SqlServer", StringComparison.OrdinalIgnoreCase);
        bool isPostgres = serverType.StartsWith("Postgres", StringComparison.OrdinalIgnoreCase);
        bool isMySql = serverType.StartsWith("MySql", StringComparison.OrdinalIgnoreCase);
        bool isSqlite = serverType.StartsWith("Sqlite", StringComparison.OrdinalIgnoreCase);
        bool isFirebird = serverType.StartsWith("Firebird", StringComparison.OrdinalIgnoreCase);

        if (isSqlite)
        {
            Directory.CreateDirectory(Path.Combine(contentRoot, "App_Data"));
            return;
        }

        var cb = DbProviderFactories.GetFactory(cs.ProviderName).CreateConnectionStringBuilder();
        cb.ConnectionString = cs.ConnectionString;

        if (isFirebird)
        {
            if (cb.ConnectionString.IndexOf(@"localhost", StringComparison.Ordinal) < 0 &&
                cb.ConnectionString.IndexOf(@"127.0.0.1", StringComparison.Ordinal) < 0)
                return;

            var database = cb["Database"] as string;
            if (string.IsNullOrEmpty(database))
                return;

            database = Path.GetFullPath(database);
            if (File.Exists(database))
                return;
            Directory.CreateDirectory(Path.GetDirectoryName(database));

            using var fbConnection = sqlConnections.New(cb.ConnectionString,
                cs.ProviderName, cs.Dialect);
            ((WrappedConnection)fbConnection).ActualConnection.GetType()
                .GetMethod("CreateDatabase", new Type[] { typeof(string), typeof(bool) })
                .Invoke(null, new object[] { fbConnection.ConnectionString, false });

            return;
        }

        if (!isSql && !isPostgres && !isMySql)
            return;

        string catalogKey = "?";

        foreach (var ck in new[] { "Initial Catalog", "Database", "AttachDBFilename" })
            if (cb.ContainsKey(ck))
            {
                catalogKey = ck;
                break;
            }

        var catalog = cb[catalogKey] as string;
        cb[catalogKey] = isPostgres ? "postgres" : null;

        using var serverConnection = sqlConnections.New(cb.ConnectionString,
            cs.ProviderName, cs.Dialect);
        serverConnection.Open();

        string databasesQuery = "SELECT * FROM sys.databases WHERE NAME = @name";
        string createDatabaseQuery = @"CREATE DATABASE [{0}]";

        if (isPostgres)
        {
            databasesQuery = "select datname from postgres.pg_catalog.pg_database where datname = @name";
            createDatabaseQuery = "CREATE DATABASE \"{0}\"";
        }
        else if (isMySql)
        {
            databasesQuery = "SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name";
            createDatabaseQuery = "CREATE DATABASE `{0}`";
        }

        if (serverConnection.Query(databasesQuery, new { name = catalog }).Any())
            return;

        var isLocalServer = isSql && (
            serverConnection.ConnectionString.Contains(@"(localdb)\", StringComparison.OrdinalIgnoreCase) ||
            serverConnection.ConnectionString.Contains(@".\", StringComparison.OrdinalIgnoreCase) ||
            serverConnection.ConnectionString.Contains(@"localhost", StringComparison.OrdinalIgnoreCase) ||
            serverConnection.ConnectionString.Contains(@"127.0.0.1", StringComparison.OrdinalIgnoreCase));

        string command;
        if (isLocalServer)
        {
            string baseDirectory = contentRoot;

            var filename = Path.Combine(Path.Combine(baseDirectory, "App_Data"), catalog);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

            command = string.Format(CultureInfo.InvariantCulture, @"CREATE DATABASE [{0}] ON PRIMARY (Name = N'{0}', FILENAME = '{1}.mdf') " +
                "LOG ON (NAME = N'{0}_log', FILENAME = '{1}.ldf')",
                catalog, filename);

            if (File.Exists(filename + ".mdf"))
                command += " FOR ATTACH";
        }
        else
        {
            command = string.Format(CultureInfo.InvariantCulture, createDatabaseQuery, catalog);
        }

        serverConnection.Execute(command);
    }
}
