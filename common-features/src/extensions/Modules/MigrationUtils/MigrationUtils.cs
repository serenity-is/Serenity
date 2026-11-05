using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Table;
using System.IO;
using System.Reflection;

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
    /// Gets the SQL dialect for the database the migration is running against. It can be used to
    /// quote identifiers and strings in the statements passed to <c>Execute.Sql</c>.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <returns>The SQL dialect.</returns>
    /// <exception cref="NotSupportedException">There is no dialect for the database type.</exception>
    public static ISqlDialect GetDialect(this MigrationBase migration)
    {
        if (migration.IsSqlServer())
            return SqlServer2000Dialect.Instance;

        if (migration.IsFirebird())
            return FirebirdDialect.Instance;

        if (migration.IsSqlite())
            return SqliteDialect.Instance;

        if (migration.IsPostgres())
            return PostgresDialect.Instance;

        if (migration.IsMySql())
            return MySqlDialect.Instance;

        if (migration.IsOracle())
            return OracleDialect.Instance;

        throw new NotSupportedException("There is no SQL dialect for the database type of this migration!");
    }

    /// <summary>
    /// Gets the column collation for the specified comparison on the database the migration is running
    /// against, in the form to pass to <c>AsString</c>.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <param name="comparer">The comparer specification, e.g. "OrdinalIgnoreCase" or "tr-TR:IgnoreCase".</param>
    /// <param name="tolerant">True to allow a collation that only differs in case or accent sensitivity.</param>
    /// <param name="exists">Optional check for whether a collation exists, e.g. the Contains method of a set of
    /// names read from the server. It is called with unquoted names, and the first candidate it accepts is used.
    /// If not specified, the first candidate is used without checking the server.</param>
    /// <returns>The collation.</returns>
    /// <exception cref="NotSupportedException">There is no such collation.</exception>
    /// <exception cref="ArgumentException">The comparer can't be mapped to a collation.</exception>
    public static string MapCollation(this MigrationBase migration, string comparer, bool tolerant = false,
        Func<string, bool>? exists = null)
    {
        var dialect = migration.GetDialect();
        return QuoteCollation(dialect, SqlCollationMapper.Map(dialect.ServerType,
            ParseCollationSpec(comparer), SqlCollationTarget.Column, tolerant, exists));
    }

    /// <summary>
    /// Tries to get the column collation for the specified comparison on the database the migration is running
    /// against, in the form to pass to <c>AsString</c>.
    /// </summary>
    /// <param name="migration">The migration reference.</param>
    /// <param name="comparer">The comparer specification, e.g. "OrdinalIgnoreCase" or "tr-TR:IgnoreCase".</param>
    /// <param name="tolerant">True to allow a collation that only differs in case or accent sensitivity.</param>
    /// <param name="exists">Optional check for whether a collation exists, e.g. the Contains method of a set of
    /// names read from the server. It is called with unquoted names, and the first candidate it accepts is used.
    /// If not specified, the first candidate is used without checking the server.</param>
    /// <returns>The collation, or null if there is no such collation, which uses the default of the database.</returns>
    /// <exception cref="ArgumentException">The comparer can't be mapped to a collation.</exception>
    public static string? TryMapCollation(this MigrationBase migration, string comparer, bool tolerant = false,
        Func<string, bool>? exists = null)
    {
        var dialect = migration.GetDialect();
        var collation = SqlCollationMapper.TryMap(dialect.ServerType,
            ParseCollationSpec(comparer), SqlCollationTarget.Column, tolerant, exists);
        return collation is null ? null : QuoteCollation(dialect, collation);
    }

    private static SqlCollationSpec ParseCollationSpec(string comparer)
    {
        return SqlCollationSpec.TryParse(comparer, out var spec) ? spec : throw new ArgumentException(
            $"Comparer '{comparer}' can't be mapped to a collation. Expected Ordinal, Invariant or a specific culture.",
            nameof(comparer));
    }

    /// <summary>
    /// FluentMigrator writes the collation as is, and the names on Postgres require quoting.
    /// </summary>
    private static string QuoteCollation(ISqlDialect dialect, string collation)
    {
        return dialect.ServerType.StartsWith("Postgres", StringComparison.OrdinalIgnoreCase)
            ? dialect.QuoteIdentifier(collation) : collation;
    }

    /// <summary>
    /// Ensures the database for the specified connection key exists, creating it if necessary.
    /// </summary>
    /// <param name="databaseKey">The connection key.</param>
    /// <param name="contentRoot">The content root path, used for local databases and Sqlite files.</param>
    /// <param name="sqlConnections">The SQL connections.</param>
    /// <param name="collation">The default collation to create the database with, in the format of the
    /// database engine, e.g. Turkish_100_CI_AS for SQL Server, utf8mb4_turkish_ci for MySQL or UNICODE_CI
    /// for Firebird. For Postgres, it is a locale like tr_TR.UTF-8, or an ICU collation like tr-TR-x-icu.
    /// It only applies when the database is created here: an existing database is not changed.
    /// Defaults to the <see cref="IConnectionString.Collation"/> of the connection, and
    /// an empty value can be passed to ignore that. It can also be one of the
    /// <see cref="SqlCollationMapper"/> keywords, which map the comparer of the connection.</param>
    /// <param name="partialContainment">True to create a SQL Server database as partially contained. This
    /// requires the "contained database authentication" setting of the server to be already enabled.
    /// Defaults to the <see cref="IConnectionString.PartialContainment"/> of the connection.</param>
    /// <exception cref="NotSupportedException">A collation is specified for a database type that
    /// can't be created with one, e.g. Sqlite or Oracle, an automap keyword can't map the comparer,
    /// or partial containment is requested for a database type other than SQL Server.</exception>
    /// <exception cref="ArgumentException">The collation has invalid characters.</exception>
    public static void EnsureDatabase(string databaseKey, string contentRoot, ISqlConnections sqlConnections,
        string? collation = null, bool? partialContainment = null)
    {
        var cs = sqlConnections.TryGetConnectionString(databaseKey)
            ?? throw new ArgumentNullException(nameof(databaseKey));
        var serverType = cs.Dialect.ServerType;
        bool isSql = serverType.StartsWith("SqlServer", StringComparison.OrdinalIgnoreCase);
        bool isPostgres = serverType.StartsWith("Postgres", StringComparison.OrdinalIgnoreCase);
        bool isMySql = serverType.StartsWith("MySql", StringComparison.OrdinalIgnoreCase);
        bool isSqlite = serverType.StartsWith("Sqlite", StringComparison.OrdinalIgnoreCase);
        bool isFirebird = serverType.StartsWith("Firebird", StringComparison.OrdinalIgnoreCase);

        var containment = "";
        if (partialContainment ?? cs.PartialContainment)
        {
            if (!isSql)
                throw new NotSupportedException($"Partial containment is not supported for {serverType}!");

            containment = " CONTAINMENT = PARTIAL";
        }

        collation = (collation ?? cs.Collation).TrimToNull();

        // collations are only checked against the server for these, where one is connected to
        var autoMap = SqlCollationMapper.IsAutoMap(collation) && (isSql || isPostgres || isMySql);
        if (!autoMap && SqlCollationMapper.IsAutoMap(collation))
            collation = SqlCollationMapper.ResolveAutoMap(collation!, cs.Comparer, serverType, SqlCollationTarget.Database);

        if (collation is not null && !autoMap)
        {
            if (!isSql && !isPostgres && !isMySql && !isFirebird)
                throw new NotSupportedException($"Creating a database with a collation is not supported for {serverType}!");

            if (!isPostgres && !collation.All(c => char.IsAsciiLetterOrDigit(c) || c == '_'))
                throw new ArgumentException($"Collation name '{collation}' has invalid characters!", nameof(collation));
        }

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

            if (collation is not null && method is not null)
            {
                // the client API has no collation option for creating a database,
                // so the default collation of the database character set is changed instead
                var charset = cb.TryGetValue("Charset", out var charsetValue) ? charsetValue as string : null;
                fbConnection.Execute($"ALTER CHARACTER SET {charset.TrimToNull() ?? "UTF8"} SET DEFAULT COLLATION {collation}");
            }

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
        string collationsQuery = "SELECT name FROM sys.fn_helpcollations()";
        string createDatabaseQuery = @"CREATE DATABASE [{0}]" + containment;

        if (isPostgres)
        {
            databasesQuery = "select datname from postgres.pg_catalog.pg_database where datname = @name";
            collationsQuery = "SELECT collname FROM pg_catalog.pg_collation";
            createDatabaseQuery = "CREATE DATABASE \"{0}\"";
        }
        else if (isMySql)
        {
            databasesQuery = "SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name";
            collationsQuery = "SELECT COLLATION_NAME FROM INFORMATION_SCHEMA.COLLATIONS";
            createDatabaseQuery = "CREATE DATABASE `{0}`";
        }

        if (serverConnection.Query(databasesQuery, new { name = catalog }).Any())
            return;

        if (autoMap)
        {
            var collations = serverConnection.Query<string>(collationsQuery).ToHashSet(StringComparer.OrdinalIgnoreCase);
            collation = SqlCollationMapper.ResolveAutoMap(collation!, cs.Comparer, serverType,
                SqlCollationTarget.Database, collations.Contains);
        }

        var isLocalDb = isSql && serverConnection.ConnectionString.Contains(@"(localdb)\", StringComparison.OrdinalIgnoreCase);

        string command;
        if (isLocalDb && !string.IsNullOrEmpty(contentRoot))
        {
            try
            {
                var filename = Path.Combine(Path.Combine(contentRoot, "App_Data"), catalog!);
                Directory.CreateDirectory(Path.GetDirectoryName(filename)!);

                // an attached database keeps its options, COLLATE and CONTAINMENT can't be used with FOR ATTACH
                command = string.Format(CultureInfo.InvariantCulture, @"CREATE DATABASE [{0}]{2} ON PRIMARY (Name = N'{0}', FILENAME = '{1}.mdf') " +
                    "LOG ON (NAME = N'{0}_log', FILENAME = '{1}.ldf')",
                    catalog, filename, File.Exists(filename + ".mdf") ? "" : containment);

                if (File.Exists(filename + ".mdf"))
                    command += " FOR ATTACH";
                else if (collation is not null)
                    command += " COLLATE " + collation;

                serverConnection.Execute(command);
                return;
            }
            catch
            {
                // ignore and try the default way
            }
        }

        command = string.Format(CultureInfo.InvariantCulture, createDatabaseQuery, catalog);
        if (collation is not null)
            command += isPostgres ? " " + GetPostgresLocaleOptions(collation, cs.Dialect) : " COLLATE " + collation;

        serverConnection.Execute(command);
    }

    /// <summary>
    /// Gets the options for creating a Postgres database with the specified collation. template0 is
    /// required, as a locale that differs from the one of the template database is not allowed otherwise.
    /// </summary>
    private static string GetPostgresLocaleOptions(string collation, ISqlDialect dialect)
    {
        const string icuSuffix = "-x-icu";

        return collation.EndsWith(icuSuffix, StringComparison.OrdinalIgnoreCase)
            ? "TEMPLATE template0 LOCALE_PROVIDER icu ICU_LOCALE " + dialect.QuoteUnicodeString(collation[..^icuSuffix.Length])
            : "TEMPLATE template0 LOCALE " + dialect.QuoteUnicodeString(collation);
    }

    /// <summary>
    /// Clears the connection pools of the database provider for the specified connection key, so that
    /// connections opened before the database was created are not reused. Does nothing if the
    /// provider is not registered, or its connection type has no static ClearAllPools method.
    /// </summary>
    /// <param name="databaseKey">The connection key.</param>
    /// <param name="sqlConnections">The SQL connections.</param>
    public static void ClearAllPools(string databaseKey, ISqlConnections sqlConnections)
    {
        ArgumentNullException.ThrowIfNull(sqlConnections);

        var cs = sqlConnections.TryGetConnectionString(databaseKey)
            ?? throw new ArgumentNullException(nameof(databaseKey));

        if (string.IsNullOrEmpty(cs.ProviderName) ||
            !DbProviderFactories.TryGetFactory(cs.ProviderName, out var factory))
            return;

        using var connection = factory.CreateConnection();
        connection?.GetType().GetMethod("ClearAllPools", BindingFlags.Public | BindingFlags.Static, Type.EmptyTypes)
            ?.Invoke(null, null);
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
