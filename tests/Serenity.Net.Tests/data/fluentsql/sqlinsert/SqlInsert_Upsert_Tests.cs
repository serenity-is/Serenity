namespace Serenity.Data;

public class SqlInsert_Upsert_Tests
{
    [Fact]
    public void ToUpsertString_FormatsForSqlServer()
    {
        var query = new SqlInsert("TestTable")
            .Dialect(SqlServer2012Dialect.Instance)
            .Set("KeyField", "x")
            .Set("DataField", "y");

        Assert.Equal(
            Normalize.Sql(
                """
                INSERT INTO [TestTable] ([KeyField], [DataField])
                SELECT @p1, @p2 WHERE NOT EXISTS (
                    SELECT 1 FROM [TestTable] WITH (UPDLOCK, SERIALIZABLE)
                    WHERE [KeyField] = @p1);
                IF @@ROWCOUNT = 0
                BEGIN
                   UPDATE [TestTable] SET [DataField] = @p2 WHERE [KeyField] = @p1;
                END
                """),
            Normalize.Sql(query.ToUpsertString(["KeyField"])));
    }

    [Fact]
    public void ToUpsertString_FormatsForSqlite()
    {
        var query = new SqlInsert("TestTable")
            .Dialect(SqliteDialect.Instance)
            .Set("KeyField", "x")
            .Set("DataField", "y");

        Assert.Equal(
            Normalize.Sql(
                """
                INSERT INTO [TestTable] ([KeyField], [DataField])
                VALUES (@p1, @p2)
                ON CONFLICT ([KeyField])
                DO UPDATE SET [DataField] = excluded.[DataField]
                """),
            Normalize.Sql(query.ToUpsertString(["KeyField"])));
    }

    [Fact]
    public void ToUpsertString_FormatsForPostgres()
    {
        var query = new SqlInsert("TestTable")
            .Dialect(PostgresDialect.Instance)
            .Set("KeyField", "x")
            .Set("DataField", "y");

        Assert.Equal(
            Normalize.Sql(
                """
                INSERT INTO [TestTable] ([KeyField], [DataField])
                VALUES (@p1, @p2)
                ON CONFLICT ([KeyField])
                DO UPDATE SET [DataField] = excluded.[DataField]
                """),
            Normalize.Sql(query.ToUpsertString(["KeyField"])));
    }

    [Fact]
    public void ToUpsertString_FormatsForMySql()
    {
        var query = new SqlInsert("TestTable")
            .Dialect(MySqlDialect.Instance)
            .Set("KeyField", "x")
            .Set("DataField", "y");

        Assert.Equal(
            Normalize.Sql(
                """
                INSERT INTO [TestTable] ([KeyField], [DataField])
                VALUES (@p1, @p2)
                ON DUPLICATE KEY UPDATE [DataField] = @p2
                """),
            Normalize.Sql(query.ToUpsertString(["KeyField"])));
    }

    [Fact]
    public void ToUpsertString_FormatsForOracle()
    {
        var query = new SqlInsert("TestTable")
            .Dialect(OracleDialect.Instance)
            .Set("KeyField", "x")
            .Set("DataField", "y");

        Assert.Equal(
            Normalize.Sql(
                """
                MERGE INTO [TestTable] t
                USING (SELECT @p1 AS [KeyField], @p2 AS [DataField] FROM dual) s
                ON (t.[KeyField] = s.[KeyField])
                WHEN MATCHED THEN
                    UPDATE SET t.[DataField] = s.[DataField]
                WHEN NOT MATCHED THEN
                    INSERT ([KeyField], [DataField]) VALUES (s.[KeyField], s.[DataField])
                """),
            Normalize.Sql(query.ToUpsertString(["KeyField"])));
    }

    [Fact]
    public void ToUpsertString_FormatsForFirebird()
    {
        var query = new SqlInsert("TestTable")
            .Dialect(FirebirdDialect.Instance)
            .Set("KeyField", "x")
            .Set("DataField", "y");

        Assert.Equal(
            Normalize.Sql(
                """
                UPDATE OR INSERT INTO [TestTable] ([KeyField], [DataField])
                VALUES (@p1, @p2)
                MATCHING ([KeyField])
                """),
            Normalize.Sql(query.ToUpsertString(["KeyField"])));
    }

    [Fact]
    public void ToUpsertString_FormatsDoNothing_WhenOnlyKeyFields()
    {
        var query = new SqlInsert("T")
            .Dialect(SqliteDialect.Instance)
            .Set("ID", 5);

        Assert.Equal(
            Normalize.Sql(
                """
                INSERT INTO [T] ([ID])
                VALUES (@p1)
                ON CONFLICT ([ID])
                DO NOTHING
                """),
            Normalize.Sql(query.ToUpsertString(["ID"])));
    }

    [Fact]
    public void FormatUpsert_ThrowsArgumentNullException_ForNullOrEmptyTable()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SqlInsert.FormatUpsert(null, ["A", "@p1"], ["A"], SqliteDialect.Instance));

        Assert.Throws<ArgumentNullException>(() =>
            SqlInsert.FormatUpsert("", ["A", "@p1"], ["A"], SqliteDialect.Instance));
    }

    [Fact]
    public void FormatUpsert_ThrowsArgumentOutOfRangeException_ForOddNameValuePairs()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SqlInsert.FormatUpsert("T", ["A", "@p1", "B"], ["A"], SqliteDialect.Instance));
    }

    [Fact]
    public void FormatUpsert_ThrowsArgumentException_ForMissingKeyField()
    {
        Assert.Throws<ArgumentException>(() =>
            SqlInsert.FormatUpsert("T", ["A", "@p1"], ["Missing"], SqliteDialect.Instance));
    }

    [Fact]
    public void FormatUpsert_ThrowsNotSupportedException_ForUnknownDialect()
    {
        Assert.Throws<NotSupportedException>(() =>
            SqlInsert.FormatUpsert("T", ["A", "@p1"], ["A"], new UnknownDialect()));
    }

    private class UnknownDialect : SqlServer2012Dialect
    {
        public override string ServerType => "Unknown";
    }
}