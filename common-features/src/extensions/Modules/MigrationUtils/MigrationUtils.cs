using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Table;
using System.IO;

namespace Serenity.Extensions;

/// <summary>
/// Helper methods for FluentMigrator migrations, including table creation
/// with identity keys and database type detection.
/// </summary>
public static class MigrationUtils
{
    /// <summary>
    /// Please prefer IdentityKey(this) on the fluent column builder
    /// </summary>
    public static void CreateTableWithId32(
        this MigrationBase migration, string table, string idField,
        Action<ICreateTableColumnOptionOrWithColumnSyntax> addColumns, string? schema = null, bool checkExists = false, bool primaryKey = true)
    {
        CreateTableWithId(migration, table, idField, addColumns, schema, 32, checkExists, primaryKey);
    }

    /// <summary>
    /// Please prefer IdentityKey(this) on the fluent column builder
    /// </summary>
    public static void CreateTableWithId64(
        this MigrationBase migration, string table, string idField,
        Action<ICreateTableColumnOptionOrWithColumnSyntax> addColumns, string? schema = null, bool checkExists = false, bool primaryKey = true)
    {
        CreateTableWithId(migration, table, idField, addColumns, schema, 64, checkExists, primaryKey);
    }

    private static void CreateTableWithId(
        MigrationBase migration, string table, string idField,
        Action<ICreateTableColumnOptionOrWithColumnSyntax> addColumns, string? schema, int size, bool checkExists = false, bool primaryKey = true)
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
    /// </summary>
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
    /// </summary>
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
            var type = builder.Column.Type is System.Data.DbType.Int64 or System.Data.DbType.UInt64 ?
                "BIGINT" : "INT";

            migration.IfDatabase("MySql").Execute.Sql(
                $"ALTER TABLE `{(string.IsNullOrEmpty(builder.SchemaName) ? "" :
                (builder.SchemaName + "."))}{builder.TableName}` MODIFY COLUMN `{builder.Column.Name}` {type} NOT NULL UNIQUE AUTO_INCREMENT;");

            return syntax;
        }

        syntax = syntax.Identity();

        return syntax;
    }

    /// <summary>
    /// Adds an Oracle sequence and trigger to generate identity values for the specified column.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <param name="table">The table name.</param>
    /// <param name="id">The identity column name.</param>
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
            .Execute.Sql(string.Format(CultureInfo.InvariantCulture, /*lang=sql*/ """
                CREATE OR REPLACE TRIGGER {2}_TRG
                BEFORE INSERT ON {0}
                FOR EACH ROW
                BEGIN
                	IF :new.{1} IS NULL THEN
                		SELECT {2}.nextval INTO :new.{1} FROM DUAL;
                	END IF;
                END;
                """, table, id, seq));

        migration.IfDatabase("Oracle")
            .Execute.Sql(@"ALTER TRIGGER " + seq + "_TRG ENABLE");
    }
    
    /// <summary>
    /// Invokes the callback only when the predicate is true, otherwise returns the syntax unchanged.
    /// </summary>
    /// <typeparam name="TSyntax">The fluent syntax type.</typeparam>
    /// <param name="syntax">The syntax builder.</param>
    /// <param name="predicate">The condition to evaluate.</param>
    /// <param name="callback">The callback to invoke when the predicate is true.</param>
    /// <returns>The syntax builder.</returns>
    public static TSyntax If<TSyntax>(this TSyntax syntax, bool predicate, Func<TSyntax, TSyntax> callback)
        where TSyntax : FluentMigrator.Infrastructure.IFluentSyntax
    {
        if (predicate)
            return callback(syntax);

        return syntax;
    }

    /// <summary>
    /// Determines whether the migration is running against a database whose type starts with the specified name.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <param name="type">The database type prefix, e.g. "SqlServer".</param>
    /// <returns><c>true</c> if the database type matches; otherwise, <c>false</c>.</returns>
    public static bool IsDatabase(this MigrationBase migration, string type)
    {
        return migration.IsDatabase(x => x.StartsWith(type, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether the migration is running against a database matching the specified predicate.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <param name="predicate">The predicate to test the database type against.</param>
    /// <returns><c>true</c> if the database type matches; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the migration is running against a Firebird database.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <returns><c>true</c> if the database is Firebird; otherwise, <c>false</c>.</returns>
    public static bool IsFirebird(this MigrationBase migration)
    {
        return IsDatabase(migration, "Firebird");
    }

    /// <summary>
    /// Determines whether the migration is running against an Oracle database.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <returns><c>true</c> if the database is Oracle; otherwise, <c>false</c>.</returns>
    public static bool IsOracle(this MigrationBase migration)
    {
        return IsDatabase(migration, "Oracle");
    }

    /// <summary>
    /// Determines whether the migration is running against a MySql database.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <returns><c>true</c> if the database is MySql; otherwise, <c>false</c>.</returns>
    public static bool IsMySql(this MigrationBase migration)
    {
        return IsDatabase(migration, "MySql");
    }

    /// <summary>
    /// Determines whether the migration is running against a Postgres database.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <returns><c>true</c> if the database is Postgres; otherwise, <c>false</c>.</returns>
    public static bool IsPostgres(this MigrationBase migration)
    {
        return IsDatabase(migration, "Postgres");
    }

    /// <summary>
    /// Determines whether the migration is running against a Sqlite database.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <returns><c>true</c> if the database is Sqlite; otherwise, <c>false</c>.</returns>
    public static bool IsSqlite(this MigrationBase migration)
    {
        return IsDatabase(migration, "Sqlite");
    }
    /// <summary>
    /// Determines whether the migration is running against a SqlServer database.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <returns><c>true</c> if the database is SqlServer; otherwise, <c>false</c>.</returns>
    public static bool IsSqlServer(this MigrationBase migration)
    {
        return IsDatabase(migration, "SqlServer");
    }

    /// <summary>
    /// Ensures the database for the specified connection key exists, creating it if necessary.
    /// </summary>
    /// <param name="databaseKey">The connection key.</param>
    /// <param name="contentRoot">The content root path, used for local databases and Sqlite files.</param>
    /// <param name="sqlConnections">The SQL connections.</param>
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
            if (!string.IsNullOrEmpty(contentRoot))
                Directory.CreateDirectory(Path.Combine(contentRoot, "App_Data"));
            return;
        }

        var cb = DbProviderFactories.GetFactory(cs.ProviderName).CreateConnectionStringBuilder()!;
        cb.ConnectionString = cs.ConnectionString;

        if (isFirebird)
        {
            if (cb.ConnectionString.IndexOf(@"localhost", StringComparison.Ordinal) < 0 &&
                cb.ConnectionString.IndexOf(@"127.0.0.1", StringComparison.Ordinal) < 0)
                return;

            var database = cb["Database"] as string;
            if (string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(Path.GetDirectoryName(database)) ||
                database.Contains(':'))
                return;

            database = Path.GetFullPath(database);
            if (File.Exists(database))
                return;
            Directory.CreateDirectory(Path.GetDirectoryName(database)!);

            using var fbConnection = sqlConnections.New(cb.ConnectionString,
                cs.ProviderName, cs.Dialect);
            var method = ((WrappedConnection)fbConnection).ActualConnection.GetType()
                .GetMethod("CreateDatabase", [typeof(string), typeof(int), typeof(bool), typeof(bool)]);

            method?.Invoke(null, [fbConnection.ConnectionString, 4096, true, false]);

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

        var isLocalDb = isSql && serverConnection.ConnectionString.Contains(@"(localdb)\", StringComparison.OrdinalIgnoreCase);

        string command;
        if (isLocalDb && !string.IsNullOrEmpty(contentRoot))
        {
            try
            {
                var filename = Path.Combine(Path.Combine(contentRoot, "App_Data"), catalog!);
                Directory.CreateDirectory(Path.GetDirectoryName(filename)!);

                command = string.Format(CultureInfo.InvariantCulture, @"CREATE DATABASE [{0}] ON PRIMARY (Name = N'{0}', FILENAME = '{1}.mdf') " +
                    "LOG ON (NAME = N'{0}_log', FILENAME = '{1}.ldf')",
                    catalog, filename);

                if (File.Exists(filename + ".mdf"))
                    command += " FOR ATTACH";

                serverConnection.Execute(command);
                return;
            }
            catch
            {
                // ignore and try the default way
            }
        }

        command = string.Format(CultureInfo.InvariantCulture, createDatabaseQuery, catalog);
        serverConnection.Execute(command);
    }

    /// <summary>
    /// Sets the column type based on the UserEntityOptions.IdFieldType, which can be Int32Field, Int64Field, StringField, or GuidField.
    /// </summary>
    /// <param name="syntax">The column syntax to set the type for.</param>
    /// <param name="userEntityOptions">User entity options, will default to Int32Field if null.</param>
    /// <exception cref="InvalidOperationException">Thrown when the UserEntityOptions.IdFieldType is not supported.</exception>
    public static ICreateTableColumnOptionOrWithColumnSyntax AsUserIdType(this ICreateTableColumnAsTypeSyntax syntax,
        IOptions<UserEntityOptions>? userEntityOptions)
    {
        var fieldType = userEntityOptions?.Value?.IdFieldType;
        if (fieldType is null || fieldType == typeof(Int32Field))
            return syntax.AsInt32();
        else if (fieldType == typeof(Int64Field))
            return syntax.AsInt64();
        else if (fieldType == typeof(StringField))
            return syntax.AsString(userEntityOptions?.Value?.IdColumnSize ?? 100);
        else if (fieldType == typeof(GuidField))
            return syntax.AsGuid();
            
        throw new InvalidOperationException("UserEntityOptions.IdFieldType is not supported: " + fieldType.FullName);
    }

    /// <summary>
    /// Sets the foreign key for a user ID column based on the UserEntityOptions, linking it to the "Users" table and "UserId" column.
    /// </summary>
    /// <typeparam name="TNext">TNext</typeparam>
    /// <typeparam name="TNextFk">TNextFk</typeparam>
    /// <param name="syntax">The column option syntax</param>
    /// <param name="userEntityOptions">User entity options</param>
    /// <param name="foreignKeyName">Foreign key name</param>
    /// <returns>The column option syntax with the foreign key applied</returns>
    public static TNextFk UserIdForeignKey<TNext, TNextFk>(this IColumnOptionSyntax<TNext, TNextFk> syntax,
        IOptions<UserEntityOptions>? userEntityOptions, string foreignKeyName)
        where TNext : FluentMigrator.Infrastructure.IFluentSyntax
        where TNextFk : FluentMigrator.Infrastructure.IFluentSyntax
    {
        return syntax.ForeignKey(foreignKeyName, userEntityOptions?.Value?.TableName ?? "Users", userEntityOptions?.Value?.IdColumnName ?? "UserId");
    }

    /// <summary>
    /// Adds a database trigger that increments this column on every update, providing optimistic
    /// concurrency support for rows implementing <c>Serenity.Data.IConcurrencyVersionRow</c>.
    /// </summary>
    /// <remarks>
    /// <para>Call on the column builder after <c>AsInt32()</c> / <c>AsInt64()</c>, e.g.
    /// <c>.WithColumn("RowVersion").AsInt32().AddConcurrencyVersionTrigger(this, idField: "ID")</c>.
    /// The column name, table name and column type (int32/int64) are read from the builder, so only
    /// the id (primary key) column is passed explicitly.</para>
    /// <para>If you only target SQL Server, prefer a native <c>rowversion</c> column (mapped as a
    /// byte[] field) which the server maintains automatically, instead of this trigger based numeric
    /// column. This trigger approach is recommended when you need to support multiple database types
    /// with the same numeric version column.</para>
    /// </remarks>
    /// <param name="syntax">Column option syntax (after AsInt32/AsInt64).</param>
    /// <param name="migration">The migration reference.</param>
    /// <param name="idField">Name of the id (primary key) column, required for the trigger.</param>
    /// <param name="schema">Optional schema name. Defaults to the table's schema.</param>
    public static ICreateTableColumnOptionOrWithColumnSyntax AddConcurrencyVersionTrigger(
        this ICreateTableColumnOptionOrWithColumnSyntax syntax,
        MigrationBase migration, string idField, string? schema = null)
    {
        var builder = (IColumnExpressionBuilder)syntax;
        AddConcurrencyVersionTriggerCore(migration, builder.TableName, builder.Column.Name,
            builder.Column.Type, idField, schema ?? builder.SchemaName);
        return syntax;
    }

    /// <summary>
    /// Adds a database trigger that increments this column on every update, providing optimistic
    /// concurrency support for rows implementing <c>Serenity.Data.IConcurrencyVersionRow</c>.
    /// </summary>
    /// <remarks>
    /// See the <see cref="AddConcurrencyVersionTrigger(ICreateTableColumnOptionOrWithColumnSyntax, MigrationBase, string, string?)"/>
    /// overload for details. This overload is for <c>Alter.Table(...).AddColumn(...)</c>.
    /// </remarks>
    /// <param name="syntax">Column option syntax (after AsInt32/AsInt64).</param>
    /// <param name="migration">The migration reference.</param>
    /// <param name="idField">Name of the id (primary key) column, required for the trigger.</param>
    /// <param name="schema">Optional schema name. Defaults to the table's schema.</param>
    public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddConcurrencyVersionTrigger(
        this IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax syntax,
        MigrationBase migration, string idField, string? schema = null)
    {
        var builder = (IColumnExpressionBuilder)syntax;
        AddConcurrencyVersionTriggerCore(migration, builder.TableName, builder.Column.Name,
            builder.Column.Type, idField, schema ?? builder.SchemaName);
        return syntax;
    }

    /// <summary>
    /// Drops the concurrency version trigger created by
    /// <see cref="AddConcurrencyVersionTrigger(ICreateTableColumnOptionOrWithColumnSyntax, MigrationBase, string, string?)"/>.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <param name="table">Table name.</param>
    /// <param name="column">Concurrency version column name.</param>
    /// <param name="schema">Optional schema name.</param>
    public static void DropConcurrencyVersionTrigger(this MigrationBase migration,
        string table, string column, string? schema = null)
    {
        ArgumentNullException.ThrowIfNull(migration);
        ArgumentException.ThrowIfNullOrEmpty(table);
        ArgumentException.ThrowIfNullOrEmpty(column);

        var name = MakeConcurrencyVersionTriggerName(table, column);

        if (migration.IsSqlServer())
            migration.IfDatabase("SqlServer").Execute.Sql($"DROP TRIGGER [{name}];");
        else if (migration.IsPostgres())
            migration.IfDatabase("Postgres").Execute.Sql($"DROP TRIGGER \"{name}\" ON {Quote(table, schema, '"', '"')}; DROP FUNCTION IF EXISTS {name}_FN();");
        else if (migration.IsMySql())
            migration.IfDatabase("MySql").Execute.Sql($"DROP TRIGGER IF EXISTS `{name}`;");
        else if (migration.IsOracle())
            migration.IfDatabase("Oracle").Execute.Sql($"DROP TRIGGER \"{name}\";");
        else if (migration.IsFirebird())
            migration.IfDatabase("Firebird").Execute.Sql($"DROP TRIGGER \"{name}\";");
        else if (migration.IsSqlite())
            migration.IfDatabase("Sqlite").Execute.Sql($"DROP TRIGGER IF EXISTS \"{name}\";");
    }

    private static string MakeConcurrencyVersionTriggerName(string table, string column)
    {
        var name = (table + "_" + column + "_CV_TRG")
            .Replace(" ", "_", StringComparison.Ordinal)
            .Replace("\"", "", StringComparison.Ordinal)
            .Replace("[", "", StringComparison.Ordinal)
            .Replace("]", "", StringComparison.Ordinal)
            .Replace("`", "", StringComparison.Ordinal);
        return name.Length > 30 ? name[..30] : name;
    }

    private static string Quote(string name, string? schema, char open, char close)
    {
        string q(string x) => open + x + close;
        return string.IsNullOrEmpty(schema) ? q(name) : q(schema) + "." + q(name);
    }

    private static void AddConcurrencyVersionTriggerCore(MigrationBase migration,
        string table, string column, System.Data.DbType? columnType, string idField, string? schema)
    {
        ArgumentNullException.ThrowIfNull(migration);
        ArgumentException.ThrowIfNullOrEmpty(table);
        ArgumentException.ThrowIfNullOrEmpty(column);
        ArgumentException.ThrowIfNullOrEmpty(idField);

        if (columnType is not (System.Data.DbType.Int32 or System.Data.DbType.Int64))
            throw new ArgumentOutOfRangeException(nameof(columnType),
                "Concurrency version column must be Int32 or Int64.");

        var serverType =
            migration.IsSqlServer() ? "SqlServer" :
            migration.IsPostgres() ? "Postgres" :
            migration.IsMySql() ? "MySql" :
            migration.IsOracle() ? "Oracle" :
            migration.IsFirebird() ? "Firebird" :
            migration.IsSqlite() ? "Sqlite" : null;

        if (serverType is null)
            throw new InvalidOperationException("Unsupported database type for a concurrency version trigger.");

        migration.IfDatabase(serverType).Execute.Sql(GetConcurrencyVersionTriggerSql(serverType,
            table, column, columnType == System.Data.DbType.Int64, idField, schema));
    }

    /// <summary>
    /// Generates the SQL statements that create the concurrency version trigger for the given
    /// database type.
    /// </summary>
    /// <param name="serverType">Database server type, e.g. "SqlServer", "Postgres", "MySql",
    /// "Oracle", "Firebird" or "Sqlite".</param>
    /// <param name="table">Table name.</param>
    /// <param name="column">Concurrency version column name.</param>
    /// <param name="isLong">True for a 64-bit (long) column, false for a 32-bit (int) column.</param>
    /// <param name="idField">Id (primary key) column name, required for SQL Server.</param>
    /// <param name="schema">Optional schema name.</param>
    /// <returns>The trigger creation SQL.</returns>
    public static string GetConcurrencyVersionTriggerSql(string serverType, string table, string column,
        bool isLong, string idField, string? schema = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(serverType);
        ArgumentException.ThrowIfNullOrEmpty(table);
        ArgumentException.ThrowIfNullOrEmpty(column);
        ArgumentException.ThrowIfNullOrEmpty(idField);

        var name = MakeConcurrencyVersionTriggerName(table, column);

        // int columns wrap around (like the common SQL Server audit triggers), long simply increments
        string Increment(string x) => isLong
            ? x + " + 1"
            : $"CASE WHEN {x} = 2147483647 THEN -2147483648 ELSE {x} + 1 END";

        if (serverType.StartsWith("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var t = Quote(table, schema, '[', ']');
            var c = $"[{column}]";
            var id = $"[{idField}]";
            return $"""
                CREATE TRIGGER [{name}] ON {t} AFTER UPDATE AS
                BEGIN
                    SET NOCOUNT ON;

                    IF NOT UPDATE({c})
                    BEGIN
                        UPDATE t SET t.{c} = {Increment("t." + c)}
                        FROM {t} t
                        INNER JOIN inserted i ON t.{id} = i.{id};
                    END
                END
                """;
        }

        if (serverType.StartsWith("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            var t = Quote(table, schema, '"', '"');
            var c = $"\"{column}\"";
            var fn = name + "_FN";
            return $"""
                CREATE OR REPLACE FUNCTION {fn}() RETURNS trigger AS $$
                BEGIN
                    NEW.{c} := {Increment("OLD." + c)};
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
                CREATE TRIGGER "{name}" BEFORE UPDATE ON {t} FOR EACH ROW EXECUTE FUNCTION {fn}();
                """;
        }

        if (serverType.StartsWith("MySql", StringComparison.OrdinalIgnoreCase))
        {
            var t = Quote(table, schema, '`', '`');
            var c = $"`{column}`";
            return $"""
                CREATE TRIGGER `{name}` BEFORE UPDATE ON {t}
                FOR EACH ROW SET NEW.{c} = {Increment("OLD." + c)};
                """;
        }

        if (serverType.StartsWith("Oracle", StringComparison.OrdinalIgnoreCase))
        {
            var t = Quote(table, schema, '"', '"');
            var c = $"\"{column}\"";
            return $"""
                CREATE OR REPLACE TRIGGER "{name}"
                BEFORE UPDATE ON {t}
                FOR EACH ROW
                BEGIN
                    :NEW.{c} := {Increment(":OLD." + c)};
                END;
                """;
        }

        if (serverType.StartsWith("Firebird", StringComparison.OrdinalIgnoreCase))
        {
            var t = Quote(table, schema, '"', '"');
            var c = $"\"{column}\"";
            return $"""
                CREATE TRIGGER "{name}" FOR {t} ACTIVE BEFORE UPDATE POSITION 0
                AS
                BEGIN
                    NEW.{c} = {Increment("OLD." + c)};
                END
                """;
        }

        if (serverType.StartsWith("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            var t = Quote(table, schema, '"', '"');
            var c = $"\"{column}\"";
            var id = $"\"{idField}\"";
            return $"""
                CREATE TRIGGER "{name}" AFTER UPDATE ON {t}
                WHEN NEW.{c} = OLD.{c}
                BEGIN
                    UPDATE {t} SET {c} = {Increment(c)} WHERE {id} = NEW.{id};
                END
                """;
        }

        throw new ArgumentOutOfRangeException(nameof(serverType), serverType,
            "Unsupported database type for a concurrency version trigger.");
    }
}
