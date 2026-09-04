namespace Serenity.Data;

public class EntityConnectionExtensions_Async_Tests
{
    // ---------- ByIdAsync / TryByIdAsync ----------

    [Fact]
    public async Task ByIdAsync_Throws_If_Record_Not_Found()
    {
        using var connection = new MockDbConnection().InterceptFindRow(_ => null);
        await Assert.ThrowsAsync<ValidationError>(
            () => connection.ByIdAsync<IdNameRow>(177, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TryByIdAsync_Returns_Null_If_Record_Not_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        Assert.Null(await connection.TryByIdAsync<IdNameRow>(278, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TryByIdAsync_Passes_The_RecordId_To_WhereStatement()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Equal(380, Assert.Single(args.Parameters).Value);

                Assert.Equal(@"SELECT 
T0.[ID] AS [ID],
T0.[Name] AS [Name] 
FROM [IdName] T0 
WHERE (T0.[ID] = @p1)".NormalizeSql(), args.CommandText.NormalizeSql());

                return new MockDbDataReader();
            });

        await connection.TryByIdAsync<IdNameRow>(380, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TryByIdAsync_Only_Loads_The_Table_Fields_By_Default()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Equal(@"SELECT 
T0.[CityId] AS [CityId],
T0.[CityName] AS [CityName],
T0.[CountryId] AS [CountryId]
FROM [Cities] T0 
WHERE (T0.[CityId] = @p1)".NormalizeSql(), args.CommandText.NormalizeSql());

                return new MockDbDataReader(new
                {
                    CityId = 481,
                    CityName = "Oslo",
                    CountryId = 482
                });
            });

        var row = await connection.TryByIdAsync<CityRow>(483, TestContext.Current.CancellationToken);
        Assert.Equal(481, row!.CityId);
        Assert.Equal("Oslo", row.CityName);
        Assert.Equal(482, row.CountryId);
        Assert.Throws<InvalidOperationException>(() => row.CountryName);
        Assert.Throws<InvalidOperationException>(() => row.FullName);
    }

    [Fact]
    public async Task TryByIdAsync_Only_Loads_The_Fields_Selected_By_Query_Callback()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Equal(@"SELECT 
jCountry.CountryName AS [CountryName]
FROM [Cities] T0 
LEFT JOIN [Countries] jCountry ON (jCountry.[CountryId] = T0.[CountryId])
WHERE (T0.[CityId] = @p1)".NormalizeSql(), args.CommandText.NormalizeSql());

                return new MockDbDataReader(new
                {
                    CountryName = "Norway"
                });
            });

        var row = await connection.TryByIdAsync<CityRow>(586, query => query
            .Select(CityRow.Fields.CountryName), TestContext.Current.CancellationToken);
        Assert.Equal("Norway", row!.CountryName);
        Assert.Throws<InvalidOperationException>(() => row.CityId);
        Assert.Throws<InvalidOperationException>(() => row.CityName);
        Assert.Throws<InvalidOperationException>(() => row.CountryId);
        Assert.Throws<InvalidOperationException>(() => row.FullName);
    }

    [Fact]
    public async Task TryByIdAsync_Exception_Thrown_If_Multiple_Results_Returned()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                return new MockDbDataReader(new
                {
                    CityId = 687,
                    CityName = "Alpha",
                    CountryId = 688
                },
                new
                {
                    CityId = 689,
                    CityName = "Beta",
                    CountryId = 690
                });
            });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => connection.TryByIdAsync<CityRow>(691, TestContext.Current.CancellationToken));
    }

    // ---------- SingleAsync / TrySingleAsync ----------

    [Fact]
    public async Task SingleAsync_Uses_Provided_WhereClause()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Contains("[CityName]", args.CommandText);
                return new MockDbDataReader(new
                {
                    CityId = 692,
                    CityName = "Lisbon",
                    CountryId = 693
                });
            });

        var row = await connection.SingleAsync<CityRow>(
            new Criteria(CityRow.Fields.CityName) == "Lisbon", TestContext.Current.CancellationToken);
        Assert.Equal("Lisbon", row!.CityName);
    }

    [Fact]
    public async Task TrySingleAsync_Returns_Null_If_None_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        Assert.Null(await connection.TrySingleAsync<CityRow>(
            new Criteria(CityRow.Fields.CityName) == "Zion", TestContext.Current.CancellationToken));
    }

    // ---------- FirstAsync / TryFirstAsync ----------

    [Fact]
    public async Task FirstAsync_Gets_First_Matching_Row()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                return new MockDbDataReader(new
                {
                    CityId = 694,
                    CityName = "Kyoto",
                    CountryId = 695
                });
            });

        var row = await connection.FirstAsync<CityRow>(
            new Criteria(CityRow.Fields.CountryId) == 697, TestContext.Current.CancellationToken);
        Assert.Equal("Kyoto", row!.CityName);
    }

    [Fact]
    public async Task TryFirstAsync_Returns_Null_If_None_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        Assert.Null(await connection.TryFirstAsync<CityRow>(
            new Criteria(CityRow.Fields.CityName) == "Eldoria", TestContext.Current.CancellationToken));
    }

    // ---------- CountAsync ----------

    [Fact]
    public async Task CountAsync_Without_Where_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(args =>
            {
                Assert.Contains("COUNT(", args.CommandText);
                return (object)441;
            });

        var count = await connection.CountAsync<CityRow>(TestContext.Current.CancellationToken);
        Assert.Equal(441, count);
    }

    [Fact]
    public async Task CountAsync_With_Where_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(args =>
            {
                Assert.Contains("COUNT(", args.CommandText);
                Assert.Contains("[CountryId]", args.CommandText);
                return (object)443;
            });

        var count = await connection.CountAsync<CityRow>(
            new Criteria(CityRow.Fields.CountryId) == 444, TestContext.Current.CancellationToken);
        Assert.Equal(443, count);
    }

    // ---------- ExistsByIdAsync / ExistsAsync ----------

    [Fact]
    public async Task ExistsByIdAsync_Returns_True_When_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Contains("[ID]", args.CommandText);
                return new MockDbDataReader(new { ID = 833 });
            });

        Assert.True(await connection.ExistsByIdAsync<IdNameRow>(844, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExistsByIdAsync_Returns_False_When_Not_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        Assert.False(await connection.ExistsByIdAsync<IdNameRow>(855, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExistsAsync_Returns_True_When_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Contains("[CityName]", args.CommandText);
                return new MockDbDataReader(new { X = 866 });
            });

        Assert.True(await connection.ExistsAsync<CityRow>(
            new Criteria(CityRow.Fields.CityName) == "Seoul", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExistsAsync_Returns_False_When_Not_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        Assert.False(await connection.ExistsAsync<CityRow>(
            new Criteria(CityRow.Fields.CityName) == "Metropolis", TestContext.Current.CancellationToken));
    }

    // ---------- ListAsync ----------

    [Fact]
    public async Task ListAsync_Lists_All_Matches()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Contains("[CityName]", args.CommandText);
                return new MockDbDataReader(new
                {
                    CityId = 877,
                    CityName = "Toronto",
                    CountryId = 878
                },
                new
                {
                    CityId = 879,
                    CityName = "Ottawa",
                    CountryId = 880
                });
            });

        var list = await connection.ListAsync<CityRow>(TestContext.Current.CancellationToken);
        Assert.Collection(list,
            city => Assert.Equal("Toronto", city.CityName),
            city => Assert.Equal("Ottawa", city.CityName));
    }

    [Fact]
    public async Task ListAsync_Supports_EditQuery()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Contains("[CountryName]", args.CommandText);
                return new MockDbDataReader(new
                {
                    CountryName = "Brazil"
                },
                new
                {
                    CountryName = "Argentina"
                });
            });

        var list = await connection.ListAsync<CityRow>(query => query
            .Select(CityRow.Fields.CountryName), TestContext.Current.CancellationToken);
        Assert.Collection(list,
            city => Assert.Equal("Brazil", city.CountryName),
            city => Assert.Equal("Argentina", city.CountryName));
    }

    // ---------- Mutation methods ----------

    [Fact]
    public async Task InsertAsync_Inserts_Row()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.StartsWith("INSERT INTO", args.CommandText);
                return 881;
            });

        var row = new IdNameRow { Name = "Sample" };
        await connection.InsertAsync(row, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InsertAndGetIDAsync_Returns_Id()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.StartsWith("INSERT INTO", args.CommandText);
                return 882;
            });

        var row = new IdNameRow { Name = "Sample" };
        var id = await connection.InsertAndGetIDAsync(row, TestContext.Current.CancellationToken);
        Assert.Equal(882, id);
    }

    [Fact]
    public async Task UpdateByIdAsync_Updates_Row()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.StartsWith("UPDATE ", args.CommandText);
                return 884;
            });

        var row = new IdNameRow { ID = 885, Name = "Renamed" };
        var affected = await connection.UpdateByIdAsync(row, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(884, affected);
    }

    [Fact]
    public async Task DeleteByIdAsync_Deletes_Row()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.StartsWith("DELETE FROM", args.CommandText);
                return 887;
            });

        var affected = await connection.DeleteByIdAsync<IdNameRow>(888, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(887, affected);
    }
}

