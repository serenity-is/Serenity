namespace Serenity.Data.Schema;

public class SchemaProvidersTests
{
    [Fact]
    public void SqlServer_DefaultSchema_Is_Dbo()
    {
        Assert.Equal("dbo", new SqlServerSchemaProvider().DefaultSchema);
    }

    [Fact]
    public void SqlServer_GetFieldInfos_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { FieldName = "ID", DataType = "int", IsNullable = false, Size = 4, Scale = 0 }));

        var fields = new SqlServerSchemaProvider().GetFieldInfos(connection, "dbo", "T").ToList();

        var field = Assert.Single(fields);
        Assert.Equal("ID", field.FieldName);
        Assert.Equal("int", field.DataType);
        Assert.False(field.IsNullable);
        Assert.Equal(4, field.Size);
    }

    [Fact]
    public void SqlServer_GetForeignKeys_Maps_ForeignKeys()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { FKName = "FK_T", FKColumn = "T_ID", PKSchema = "dbo", PKTable = "T", PKColumn = "ID" }));

        var fk = Assert.Single(new SqlServerSchemaProvider().GetForeignKeys(connection, "dbo", "T"));
        Assert.Equal("FK_T", fk.FKName);
        Assert.Equal("T_ID", fk.FKColumn);
        Assert.Equal("T", fk.PKTable);
        Assert.Equal("ID", fk.PKColumn);
    }

    [Fact]
    public void SqlServer_GetIdentityFields_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(new { COLUMN_NAME = "ID" }));

        Assert.Equal(["ID"], new SqlServerSchemaProvider().GetIdentityFields(connection, "dbo", "T").ToList());
    }

    [Fact]
    public void SqlServer_GetPrimaryKeyFields_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(new { COLUMN_NAME = "ID" }));

        Assert.Equal(["ID"], new SqlServerSchemaProvider().GetPrimaryKeyFields(connection, "dbo", "T").ToList());
    }

    [Fact]
    public void SqlServer_GetTableNames_Maps_Tables_And_Views()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { TABLE_SCHEMA = "dbo", TABLE_NAME = "T", TABLE_TYPE = "BASE TABLE" },
                new { TABLE_SCHEMA = "dbo", TABLE_NAME = "V", TABLE_TYPE = "VIEW" }));

        var tables = new SqlServerSchemaProvider().GetTableNames(connection).ToList();

        Assert.Equal(2, tables.Count);
        Assert.Equal("dbo", tables[0].Schema);
        Assert.Equal("T", tables[0].Table);
        Assert.False(tables[0].IsView);
        Assert.True(tables[1].IsView);
    }

    [Fact]
    public void Postgres_DefaultSchema_Is_Public()
    {
        Assert.Equal("public", new PostgresSchemaProvider().DefaultSchema);
    }

    [Fact]
    public void Postgres_GetFieldInfos_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { FieldName = "ID", DataType = "integer", IsNullable = false, IsIdentity = true, Size = 0, Scale = 0 }));

        var field = Assert.Single(new PostgresSchemaProvider().GetFieldInfos(connection, "public", "T"));
        Assert.Equal("ID", field.FieldName);
        Assert.True(field.IsIdentity);
    }

    [Fact]
    public void Postgres_GetForeignKeys_Maps_ForeignKeys()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { FKName = "FK_T", FKColumn = "T_ID", PKSchema = "public", PKTable = "T", PKColumn = "ID" }));

        var fk = Assert.Single(new PostgresSchemaProvider().GetForeignKeys(connection, "public", "T"));
        Assert.Equal("FK_T", fk.FKName);
    }

    [Fact]
    public void Postgres_GetIdentityFields_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { column_name = "ID", column_default = "nextval('T_ID_seq')" }));

        Assert.Equal(["ID"], new PostgresSchemaProvider().GetIdentityFields(connection, "public", "T").ToList());
    }

    [Fact]
    public void Postgres_GetPrimaryKeyFields_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(new { attname = "ID" }));

        Assert.Equal(["ID"], new PostgresSchemaProvider().GetPrimaryKeyFields(connection, "public", "T").ToList());
    }

    [Fact]
    public void Postgres_GetTableNames_Maps_Tables_And_Views()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { table_schema = "public", table_name = "T", table_type = "BASE TABLE" },
                new { table_schema = "public", table_name = "V", table_type = "VIEW" }));

        var tables = new PostgresSchemaProvider().GetTableNames(connection).ToList();

        Assert.Equal("public", tables[0].Schema);
        Assert.Equal("T", tables[0].Table);
        Assert.False(tables[0].IsView);
        Assert.True(tables[1].IsView);
    }

    [Fact]
    public void MySql_DefaultSchema_Is_Null()
    {
        Assert.Null(new MySqlSchemaProvider().DefaultSchema);
    }

    [Fact]
    public void MySql_GetFieldInfos_Parses_Types_And_Keys()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { ORDINAL_POSITION = "1", Field = "Name", Null = "NO", Type = "varchar(50)", Key = "PRI", Extra = "auto_increment" },
                new { ORDINAL_POSITION = "2", Field = "Amount", Null = "YES", Type = "decimal(10,2)", Key = "", Extra = "" }));

        var fields = new MySqlSchemaProvider().GetFieldInfos(connection, "db", "T").ToList();

        Assert.Equal("varchar", fields[0].DataType);
        Assert.Equal(50, fields[0].Size);
        Assert.False(fields[0].IsNullable);
        Assert.True(fields[0].IsPrimaryKey);
        Assert.True(fields[0].IsIdentity);

        Assert.Equal("decimal", fields[1].DataType);
        Assert.Equal(10, fields[1].Size);
        Assert.Equal(2, fields[1].Scale);
        Assert.True(fields[1].IsNullable);
    }

    [Fact]
    public void MySql_GetForeignKeys_Maps_ForeignKeys()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { FKName = "FK_T", FKColumn = "T_ID", PKSchema = "other", PKTable = "T", PKColumn = "ID" }));

        var fk = Assert.Single(new MySqlSchemaProvider().GetForeignKeys(connection, "db", "T"));
        Assert.Equal("other", fk.PKSchema);
    }

    [Fact]
    public void MySql_GetIdentityFields_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(new { COLUMN_NAME = "ID" }));

        Assert.Equal(["ID"], new MySqlSchemaProvider().GetIdentityFields(connection, "db", "T").ToList());
    }

    [Fact]
    public void MySql_GetPrimaryKeyFields_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(new { COLUMN_NAME = "ID" }));

        Assert.Equal(["ID"], new MySqlSchemaProvider().GetPrimaryKeyFields(connection, "db", "T").ToList());
    }

    [Fact]
    public void MySql_GetTableNames_Maps_Tables_And_Views()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { TABLE_NAME = "T", TABLE_TYPE = "BASE TABLE" },
                new { TABLE_NAME = "V", TABLE_TYPE = "VIEW" }));

        var tables = new MySqlSchemaProvider().GetTableNames(connection).ToList();
        Assert.False(tables[0].IsView);
        Assert.True(tables[1].IsView);
    }

    [Fact]
    public void Oracle_DefaultSchema_Is_Null()
    {
        Assert.Null(new OracleSchemaProvider().DefaultSchema);
    }

    [Fact]
    public void Oracle_GetFieldInfos_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { FieldName = "ID", DataType = "NUMBER", Size = 4, Scale = 0, IsNullable = false }));

        var field = Assert.Single(new OracleSchemaProvider().GetFieldInfos(connection, "dbo", "T"));
        Assert.Equal("ID", field.FieldName);
        Assert.Equal("NUMBER", field.DataType);
    }

    [Fact]
    public void Oracle_GetForeignKeys_Maps_ForeignKeys()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { FKName = "FK_T", FKColumn = "T_ID", PKSchema = "dbo", PKTable = "T", PKColumn = "ID" }));

        Assert.Single(new OracleSchemaProvider().GetForeignKeys(connection, "dbo", "T"));
    }

    [Fact]
    public void Oracle_GetIdentityFields_Is_Empty()
    {
        using var connection = new MockDbConnection();
        Assert.Empty(new OracleSchemaProvider().GetIdentityFields(connection, "dbo", "T"));
    }

    [Fact]
    public void Oracle_GetPrimaryKeyFields_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(new { COLUMN_NAME = "ID" }));

        Assert.Equal(["ID"], new OracleSchemaProvider().GetPrimaryKeyFields(connection, "dbo", "T").ToList());
    }

    [Fact]
    public void Oracle_GetTableNames_Maps_Tables()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(new { Schema = "dbo", Table = "T" }));

        var table = Assert.Single(new OracleSchemaProvider().GetTableNames(connection));
        Assert.Equal("dbo", table.Schema);
        Assert.Equal("T", table.Table);
    }

    [Fact]
    public void Sqlite_DefaultSchema_Is_Null()
    {
        Assert.Null(new SqliteSchemaProvider().DefaultSchema);
    }

    [Fact]
    public void Sqlite_GetFieldInfos_Maps_Fields()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { name = "Id", type = "INTEGER", notnull = "1", pk = "1" },
                new { name = "Name", type = "TEXT", notnull = "0", pk = "0" }));

        var fields = new SqliteSchemaProvider().GetFieldInfos(connection, null, "T").ToList();

        Assert.Equal("Id", fields[0].FieldName);
        Assert.Equal("INTEGER", fields[0].DataType);
        Assert.False(fields[0].IsNullable);
        Assert.True(fields[0].IsPrimaryKey);
        Assert.True(fields[1].IsNullable);
        Assert.False(fields[1].IsPrimaryKey);
    }

    [Fact]
    public void Sqlite_GetForeignKeys_Maps_ForeignKeys()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new[]
                {
                    new Dictionary<string, object>
                    {
                        ["id"] = "1",
                        ["from"] = "T_ID",
                        ["table"] = "T",
                        ["to"] = "ID"
                    }
                },
                "id", "from", "table", "to"));

        var fk = Assert.Single(new SqliteSchemaProvider().GetForeignKeys(connection, null, "T"));
        Assert.Equal("1", fk.FKName);
        Assert.Equal("T_ID", fk.FKColumn);
        Assert.Equal("T", fk.PKTable);
        Assert.Equal("ID", fk.PKColumn);
    }

    [Fact]
    public void Sqlite_GetIdentityFields_Returns_Integer_PrimaryKey()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { pk = 1, name = "Id", type = "INTEGER" }));

        Assert.Equal(["Id"], new SqliteSchemaProvider().GetIdentityFields(connection, null, "T").ToList());
    }

    [Fact]
    public void Sqlite_GetIdentityFields_Returns_RowId_When_Not_Integer()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { pk = 1, name = "Id", type = "TEXT" }));

        Assert.Equal(["ROWID"], new SqliteSchemaProvider().GetIdentityFields(connection, null, "T").ToList());
    }

    [Fact]
    public void Sqlite_GetIdentityFields_Returns_RowId_When_No_PrimaryKey()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader());

        Assert.Equal(["ROWID"], new SqliteSchemaProvider().GetIdentityFields(connection, null, "T").ToList());
    }

    [Fact]
    public void Sqlite_GetPrimaryKeyFields_Orders_By_Pk()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { pk = 2, name = "B" },
                new { pk = 1, name = "A" }));

        Assert.Equal(["A", "B"], new SqliteSchemaProvider().GetPrimaryKeyFields(connection, null, "T").ToList());
    }

    [Fact]
    public void Sqlite_GetTableNames_Maps_Tables_And_Views()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { name = "T", type = "table" },
                new { name = "V", type = "view" }));

        var tables = new SqliteSchemaProvider().GetTableNames(connection).ToList();
        Assert.Equal("T", tables[0].Table);
        Assert.False(tables[0].IsView);
        Assert.True(tables[1].IsView);
    }

    [Fact]
    public void Firebird_DefaultSchema_Is_Null()
    {
        Assert.Null(new FirebirdSchemaProvider().DefaultSchema);
    }

    [Fact]
    public void Firebird_GetFieldInfos_Maps_Char_And_Decimal()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new
                {
                    FIELD_NAME = "Name",
                    FIELD_TYPE = "37",
                    FIELD_SUB_TYPE = "0",
                    NUMERIC_SCALE = "0",
                    NUMERIC_PRECISION = "0",
                    SIZE = "50",
                    CHARMAXLENGTH = "50",
                    COLUMN_NULLABLE = (string)null
                },
                new
                {
                    FIELD_NAME = "Amount",
                    FIELD_TYPE = "8",
                    FIELD_SUB_TYPE = "2",
                    NUMERIC_SCALE = "2",
                    NUMERIC_PRECISION = "10",
                    SIZE = "10",
                    CHARMAXLENGTH = (string)null,
                    COLUMN_NULLABLE = "1"
                },
                new
                {
                    FIELD_NAME = "Content",
                    FIELD_TYPE = "261",
                    FIELD_SUB_TYPE = "1",
                    NUMERIC_SCALE = "0",
                    NUMERIC_PRECISION = "0",
                    SIZE = "0",
                    CHARMAXLENGTH = (string)null,
                    COLUMN_NULLABLE = (string)null
                }));

        var fields = new FirebirdSchemaProvider().GetFieldInfos(connection, null, "T").ToList();

        Assert.Equal("Name", fields[0].FieldName);
        Assert.Equal("varchar", fields[0].DataType);
        Assert.Equal(50, fields[0].Size);
        Assert.True(fields[0].IsNullable);

        Assert.Equal("decimal", fields[1].DataType);
        Assert.Equal(10, fields[1].Size);
        Assert.Equal(-2, fields[1].Scale);
        Assert.False(fields[1].IsNullable);

        Assert.Equal("text", fields[2].DataType);
        Assert.Equal(0, fields[2].Size);
    }

    [Fact]
    public void Firebird_GetForeignKeys_Trims_Values()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { PKTable = " T ", PKColumn = " ID ", FKName = " FK " }));

        var fk = Assert.Single(new FirebirdSchemaProvider().GetForeignKeys(connection, null, "T"));
        Assert.Equal("FK", fk.FKName);
        Assert.Equal("T", fk.PKTable);
        Assert.Equal("ID", fk.PKColumn);
        Assert.Equal("", fk.FKColumn);
    }

    [Fact]
    public void Firebird_GetIdentityFields_Uses_Generator()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => cmd.CommandText.Contains("RDB$GENERATORS")
                ? new MockDbDataReader(new { VALUE = "GEN_T_ID" })
                : new MockDbDataReader(new { VALUE = "ID" }));

        Assert.Equal(["ID"], new FirebirdSchemaProvider().GetIdentityFields(connection, null, "T").ToList());
    }

    [Fact]
    public void Firebird_GetIdentityFields_Falls_Back_To_Single_PrimaryKey()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => cmd.CommandText.Contains("RDB$GENERATORS")
                ? new MockDbDataReader()
                : new MockDbDataReader(new { VALUE = "ID" }));

        Assert.Equal(["ID"], new FirebirdSchemaProvider().GetIdentityFields(connection, null, "T").ToList());
    }

    [Fact]
    public void Firebird_GetIdentityFields_Returns_Empty_For_Composite_PrimaryKey()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => cmd.CommandText.Contains("RDB$GENERATORS")
                ? new MockDbDataReader()
                : new MockDbDataReader(
                    new { VALUE = "A" },
                    new { VALUE = "B" }));

        Assert.Empty(new FirebirdSchemaProvider().GetIdentityFields(connection, null, "T"));
    }

    [Fact]
    public void Firebird_GetPrimaryKeyFields_Trims_Values()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { VALUE = " ID " },
                new { VALUE = " " }));

        Assert.Equal(["ID"], new FirebirdSchemaProvider().GetPrimaryKeyFields(connection, null, "T").ToList());
    }

    [Fact]
    public void Firebird_GetTableNames_Maps_Tables_And_Views()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd => new MockDbDataReader(
                new { NAME = " T ", ISVIEW = (string)null },
                new { NAME = "V", ISVIEW = "x" }));

        var tables = new FirebirdSchemaProvider().GetTableNames(connection).ToList();
        Assert.Equal("T", tables[0].Table);
        Assert.False(tables[0].IsView);
        Assert.True(tables[1].IsView);
    }

    [Theory]
    [InlineData(37, 0, 0, 0, "varchar")]
    [InlineData(38, 0, 0, 0, "varchar")]
    [InlineData(14, 0, 16, 0, "guid")]
    [InlineData(14, 0, 10, 0, "char")]
    [InlineData(15, 0, 10, 0, "char")]
    [InlineData(40, 0, 10, 0, "char")]
    [InlineData(41, 0, 10, 0, "char")]
    [InlineData(7, 2, 0, 0, "decimal")]
    [InlineData(7, 1, 0, 0, "numeric")]
    [InlineData(7, 0, 0, -1, "decimal")]
    [InlineData(7, 0, 0, 0, "smallint")]
    [InlineData(8, 2, 0, 0, "decimal")]
    [InlineData(8, 1, 0, 0, "numeric")]
    [InlineData(8, 0, 0, -1, "decimal")]
    [InlineData(8, 0, 0, 0, "integer")]
    [InlineData(9, 0, 0, 0, "bigint")]
    [InlineData(9, 2, 0, 0, "decimal")]
    [InlineData(9, 1, 0, 0, "numeric")]
    [InlineData(9, 0, 0, -1, "decimal")]
    [InlineData(16, 0, 0, 0, "bigint")]
    [InlineData(45, 0, 0, 0, "bigint")]
    [InlineData(10, 0, 0, 0, "float")]
    [InlineData(27, 0, 0, 0, "double")]
    [InlineData(27, 2, 0, 0, "decimal")]
    [InlineData(27, 1, 0, 0, "numeric")]
    [InlineData(27, 0, 0, -1, "decimal")]
    [InlineData(11, 0, 0, 0, "double")]
    [InlineData(261, 1, 0, 0, "text")]
    [InlineData(261, 0, 0, 0, "varbinary")]
    [InlineData(35, 0, 0, 0, "datetime")]
    [InlineData(13, 0, 0, 0, "time")]
    [InlineData(12, 0, 0, 0, "date")]
    [InlineData(23, 0, 0, 0, "boolean")]
    [InlineData(999, 0, 0, 0, "unknown")]
    public void Firebird_GetSqlTypeFromBlrType_Returns_Expected(int type, int subType, int size, int scale, string expected)
    {
        Assert.Equal(expected, FirebirdSchemaProvider.GetSqlTypeFromBlrType(type, subType, size, scale));
    }
}
