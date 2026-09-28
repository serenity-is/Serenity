namespace Serenity.Data;

using Serenity.Data.Mapping;

public class EntitySqlQueryProjection_Tests
{
    [Fact]
    public void ListProjectedMapsFlatAnonymousProjectionWithoutIntoFieldTargets()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { PersonID = 17 });
            });

        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row);
        var extensible = (ISqlQueryExtensible)query;
        var currentIntoRow = extensible.CurrentIntoRow;
        var result = query.ListProjected(connection,
            (SelfNavigationRow source) => new { PersonID = source.ID }).Single();

        Assert.Equal(17, result.PersonID);
        Assert.Contains("AS [PersonID]", commandText);
        Assert.Empty(extensible.Columns);
        Assert.Same(currentIntoRow, extensible.CurrentIntoRow);
    }

    [Fact]
    public void QueryProjectedBuffersByDefault()
    {
        MockDbDataReader? dataReader = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => dataReader = new MockDbDataReader(new { ID = 17 }));

        var query = new SqlQuery().From(new SelfNavigationRow());
        var result = query.QueryProjected(connection,
            (SelfNavigationRow source) => new { ID = source.ID });

        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
        Assert.True(dataReader!.IsClosed);
        Assert.Equal(17, Assert.Single(result).ID);
    }

    [Fact]
    public void ListProjectedMapsCapturedSqlExpression()
    {
        string? commandText = null;
        var expression = "CAST(17 AS INT)";
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { SomeProp = 17 });
            });

        var query = new SqlQuery().From(new SelfNavigationRow());
        var result = query.ListProjected(connection,
            (SelfNavigationRow source) => new { SomeProp = Sql.Expr<int?>(expression) }).Single();

        Assert.Equal((int?)17, result.SomeProp);
        Assert.Contains("CAST(17 AS INT) AS [SomeProp]", commandText);
    }

    [Fact]
    public void ReusableProjectedQueryReusesPreparedQueryAndOverridesParams()
    {
        var parameterValues = new List<object?>();
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                parameterValues.Add(((System.Data.Common.DbParameter)command.Parameters[0]).Value);
                return new MockDbDataReader(new { ID = 17 });
            });

        var query = new SqlQuery().From(new SelfNavigationRow());
        query.SetParam("p1", 1);
        var extensible = (ISqlQueryExtensible)query;
        var currentIntoRow = extensible.CurrentIntoRow;
        var projected = query.AsReusableProjected(
            (SelfNavigationRow source) => new { ID = source.ID }, [SelfNavigationRow.Fields]);

        Assert.Equal(17, Assert.Single(projected.List(connection)).ID);
        Assert.Equal(17, Assert.Single(projected.List(connection,
            new Dictionary<string, object?> { ["p1"] = 2 })).ID);
        Assert.Equal(17, Assert.Single(projected.List(connection)).ID);

        Assert.Equal(new object?[] { 1, 2, 1 }, parameterValues);
        Assert.Equal(1, query.Params!["p1"]);
        Assert.Empty(extensible.Columns);
        Assert.Same(currentIntoRow, extensible.CurrentIntoRow);
        Assert.Equal(17, Assert.Single(projected.Query(connection, buffered: false)).ID);
    }

    [Fact]
    public async Task ReusableProjectedQueryStreamsAsync()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 17 }, new { ID = 23 }));

        var projected = new SqlQuery()
            .From(new SelfNavigationRow())
            .AsReusableProjected((SelfNavigationRow source) => new { ID = source.ID });
        var ids = new List<int?>();

        await foreach (var result in projected.QueryAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken))
            ids.Add(result.ID);

        Assert.Equal(new int?[] { 17, 23 }, ids);
    }

    [Fact]
    public async Task ReusableProjectedQueryAllowsParallelExecutions()
    {
        var query = new SqlQuery().From(new SelfNavigationRow());
        query.SetParam("p1", 0);
        var projected = query.AsReusableProjected((SelfNavigationRow source) => new { ID = source.ID });

        var executions = await Task.WhenAll(new[] { 1, 2 }.Select(value => Task.Run(() =>
        {
            var observedParameter = 0;
            using var connection = new MockDbConnection()
                .OnDbCommandExecuteReader(command =>
                {
                    observedParameter = Convert.ToInt32(
                        ((System.Data.Common.DbParameter)command.Parameters[0]).Value);
                    return new MockDbDataReader(new { ID = 17 });
                });

            var result = Assert.Single(projected.List(connection,
                new Dictionary<string, object?> { ["p1"] = value }));
            return (observedParameter, result.ID);
        })));

        Assert.Equal(new[] { 1, 2 }, executions.Select(execution => execution.observedParameter).Order());
        Assert.All(executions, execution => Assert.Equal(17, execution.ID));
        Assert.Equal(0, query.Params!["p1"]);
    }

    [Fact]
    public void QueryProjectedUnbufferedStartsOnEnumerationAndDisposesReaderOnEarlyExit()
    {
        MockDbDataReader? dataReader = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => dataReader = new MockDbDataReader(
                new { ID = 17 }, new { ID = 23 }));

        var query = new SqlQuery().From(new SelfNavigationRow());
        var result = query.QueryProjected(connection,
            (SelfNavigationRow source) => new { ID = source.ID },
            [SelfNavigationRow.Fields], buffered: false);

        Assert.Equal(0, connection.DbCommandExecuteReaderCallCount);
        using (var enumerator = result.GetEnumerator())
        {
            Assert.True(enumerator.MoveNext());
            Assert.Equal(17, enumerator.Current.ID);
            Assert.False(dataReader!.IsClosed);
        }

        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
        Assert.True(dataReader!.IsClosed);
    }

    [Fact]
    public async Task ListProjectedAsyncBuffersResultsAndDisposesReader()
    {
        MockDbDataReader? dataReader = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => dataReader = new MockDbDataReader(
                new { ID = 17 }, new { ID = 23 }));

        var query = new SqlQuery().From(new SelfNavigationRow());
        var result = await query.ListProjectedAsync(connection,
            (SelfNavigationRow source) => new { ID = source.ID },
            [SelfNavigationRow.Fields],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(new int?[] { 17, 23 }, result.Select(item => item.ID));
        Assert.True(dataReader!.IsClosed);
    }

    [Fact]
    public async Task QueryProjectedAsyncStreamsAndDisposesReaderOnEarlyExit()
    {
        MockDbDataReader? dataReader = null;
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return dataReader = new MockDbDataReader(new { FirstID = 17, SecondID = 23 });
            });

        var first = new SelfNavigationRow();
        var secondFields = SelfNavigationRow.Fields.As("T1");
        var query = new SqlQuery().From(first)
            .From(secondFields);
        var results = query.QueryProjectedAsync(connection,
            (SelfNavigationRow firstSource, SelfNavigationRow secondSource) =>
                new { FirstID = firstSource.ID, SecondID = secondSource.ID },
            [SelfNavigationRow.Fields, secondFields],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(0, connection.DbCommandExecuteReaderCallCount);
        await foreach (var result in results)
        {
            Assert.Equal(17, result.FirstID);
            Assert.Equal(23, result.SecondID);
            break;
        }

        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
        Assert.True(dataReader!.IsClosed);
        Assert.Contains("T0.[ID] AS [FirstID]", commandText);
        Assert.Contains("T1.[ID] AS [SecondID]", commandText);
    }

    [Fact]
    public void ListProjectedResolvesForeignRowFieldPaths()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { ManagerID = 23 });
            });

        var query = new SqlQuery().From(new SelfNavigationRow());
        var result = query.ListProjected(connection,
            (SelfNavigationRow source) => new { ManagerID = source.Manager!.ID }).Single();

        Assert.Equal(23, result.ManagerID);
        Assert.Contains("jManager", commandText);
        Assert.Contains("LEFT JOIN", commandText);
        Assert.Contains("AS [ManagerID]", commandText);
    }

    [Fact]
    public void ListProjectedRejectsIntoRowTypeMismatch()
    {
        using var connection = new MockDbConnection();
        var query = new SqlQuery().From(new ComplexRow());

        Assert.Throws<InvalidOperationException>(() => query.ListProjected(connection,
            (SelfNavigationRow source) => new { PersonID = source.ID }));
    }

    [Fact]
    public void ListProjectedSupportsTwoSourceRowsAndFlatObjectInitializers()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { FirstID = 17, SecondID = 23 });
            });

        var first = new SelfNavigationRow();
        var secondFields = SelfNavigationRow.Fields.As("T1");
        var query = new SqlQuery().From(first)
            .From(secondFields);

        var result = query.ListProjected(connection,
            (SelfNavigationRow firstSource, SelfNavigationRow secondSource) =>
                new FlatProjectionResult
                {
                    FirstID = firstSource.ID,
                    SecondID = secondSource.ID
                }, [SelfNavigationRow.Fields, secondFields]).Single();

        Assert.Equal(17, result.FirstID);
        Assert.Equal(23, result.SecondID);
        Assert.Contains("T0.[ID] AS [FirstID]", commandText);
        Assert.Contains("T1.[ID] AS [SecondID]", commandText);
    }

    [Fact]
    public void ListProjectedSupportsThreeSourceRows()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { FirstID = 17, SecondID = 23, ThirdID = 31 });
            });

        var first = new SelfNavigationRow();
        var secondFields = SelfNavigationRow.Fields.As("T1");
        var thirdFields = SelfNavigationRow.Fields.As("T2");
        var query = new SqlQuery().From(first)
            .From(secondFields)
            .From(thirdFields);

        var result = query.ListProjected(connection,
            (SelfNavigationRow firstSource, SelfNavigationRow secondSource, SelfNavigationRow thirdSource) =>
                new { FirstID = firstSource.ID, SecondID = secondSource.ID, ThirdID = thirdSource.ID },
            [SelfNavigationRow.Fields, secondFields, thirdFields]).Single();

        Assert.Equal(17, result.FirstID);
        Assert.Equal(23, result.SecondID);
        Assert.Equal(31, result.ThirdID);
        Assert.Contains("T0.[ID] AS [FirstID]", commandText);
        Assert.Contains("T1.[ID] AS [SecondID]", commandText);
        Assert.Contains("T2.[ID] AS [ThirdID]", commandText);
    }

    [Fact]
    public void ListProjectedInfersUniqueTypedJoinSourceWithoutInto()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { ManagerID = 23 });
            });

        var managerFields = SelfNavigationRow.Fields.As("Manager");
        var query = new SqlQuery().From(new ComplexRow())
            .LeftJoin(managerFields,
                new Criteria("Manager", "ID") == new Criteria(0, "ComplexID"));

        var result = query.ListProjected(connection,
            (SelfNavigationRow manager) => new { ManagerID = manager.ID }).Single();

        Assert.Equal(23, result.ManagerID);
        Assert.Contains("LEFT JOIN [SelfNavigation] Manager", commandText);
        Assert.Contains("Manager.[ID] AS [ManagerID]", commandText);
    }

    [Fact]
    public void ListProjectedInfersSourcesFromRowAndAliasedFieldsWithoutInto()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { RootID = 17, JoinedID = 23 });
            });

        var joinedFields = SelfNavigationRow.Fields.As("T1");
        var query = new SqlQuery().From(new ComplexRow())
            .From(joinedFields);

        var result = query.ListProjected(connection,
            (ComplexRow root, SelfNavigationRow joined) =>
                new { RootID = root.ID, JoinedID = joined.ID }).Single();

        Assert.Equal(17, result.RootID);
        Assert.Equal(23, result.JoinedID);
        Assert.Contains("T0.[ComplexID] AS [RootID]", commandText);
        Assert.Contains("T1.[ID] AS [JoinedID]", commandText);
    }

    [Fact]
    public void ListProjectedInfersSourcesFromFieldsOnlyWithoutInto()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { RootID = 17, JoinedID = 23 });
            });

        var rootFields = ComplexRow.Fields;
        var joinedFields = SelfNavigationRow.Fields.As("T1");
        var query = new SqlQuery().From(rootFields).From(joinedFields);

        var result = query.ListProjected(connection,
            (ComplexRow root, SelfNavigationRow joined) =>
                new { RootID = root.ID, JoinedID = joined.ID }).Single();

        Assert.Equal(17, result.RootID);
        Assert.Equal(23, result.JoinedID);
        Assert.Contains("T0.[ComplexID] AS [RootID]", commandText);
        Assert.Contains("T1.[ID] AS [JoinedID]", commandText);
    }

    [Fact]
    public void ListProjectedRejectsAmbiguousInferenceAndAllowsExplicitSources()
    {
        string? commandText = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command =>
            {
                commandText = command.CommandText;
                return new MockDbDataReader(new { FirstID = 17, SecondID = 23 });
            });

        var secondFields = SelfNavigationRow.Fields.As("T1");
        var query = new SqlQuery().From(new SelfNavigationRow())
            .InnerJoin(secondFields,
                new Criteria("T1", "ID") == new Criteria(0, "ID"));

        Assert.Throws<InvalidOperationException>(() => query.ListProjected(connection,
            (SelfNavigationRow source) => new { ID = source.ID }));

        var result = query.ListProjected(connection,
            (SelfNavigationRow firstSource, SelfNavigationRow secondSource) =>
                new { FirstID = firstSource.ID, SecondID = secondSource.ID },
            [SelfNavigationRow.Fields, secondFields]).Single();

        Assert.Equal(17, result.FirstID);
        Assert.Equal(23, result.SecondID);
        Assert.Contains("T0.[ID] AS [FirstID]", commandText);
        Assert.Contains("T1.[ID] AS [SecondID]", commandText);
    }

    public sealed class FlatProjectionResult
    {
        public int? FirstID { get; set; }
        public int? SecondID { get; set; }
    }

    [TableName("SelfNavigation")]
    public sealed class SelfNavigationRow : Row<SelfNavigationRow.RowFields>
    {
        private const string jManager = nameof(jManager);
        private const string jMentor = nameof(jMentor);

        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [ForeignKey(typeof(SelfNavigationRow), nameof(ID)), LeftJoin(jManager)]
        public int? ManagerID { get => fields.ManagerID[this]; set => fields.ManagerID[this] = value; }

        [ForeignRow(nameof(ManagerID))]
        public SelfNavigationRow? Manager { get => fields.Manager[this]; set => fields.Manager[this] = value; }

        [ForeignKey(typeof(SelfNavigationRow), nameof(ID)), LeftJoin(jMentor)]
        public int? MentorID { get => fields.MentorID[this]; set => fields.MentorID[this] = value; }

        [ForeignRow(nameof(MentorID))]
        public SelfNavigationRow? Mentor { get => fields.Mentor[this]; set => fields.Mentor[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public Int32Field ManagerID = null!;
            public RowField<SelfNavigationRow> Manager = null!;
            public Int32Field MentorID = null!;
            public RowField<SelfNavigationRow> Mentor = null!;
        }
    }
}