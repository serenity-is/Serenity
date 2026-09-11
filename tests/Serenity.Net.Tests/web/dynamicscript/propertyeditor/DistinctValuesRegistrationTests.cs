namespace Serenity.Web;

public class DistinctValuesRegistrationTests
{
    private class DistinctRow : Row<DistinctRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [DistinctValuesEditor]
        public string? Category { get => fields.Category[this]; set => fields.Category[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Category;
#pragma warning restore CS0649
        }
    }

    [FormScript("Registration.BasedForm")]
    [BasedOnRow(typeof(DistinctRow))]
    private class BasedForm
    {
        [DistinctValuesEditor]
        public string? Category { get; set; }
    }

    [FormScript("Registration.ExplicitForm")]
    private class ExplicitForm
    {
        [DistinctValuesEditor(typeof(DistinctRow), "Category")]
        public string? Category { get; set; }
    }

    [FormScript("Registration.BadRowForm")]
    private class BadRowForm
    {
        [DistinctValuesEditor(typeof(string), "Category")]
        public string? Category { get; set; }
    }

    [FormScript("Registration.NoRowForm")]
    private class NoRowForm
    {
        [DistinctValuesEditor]
        public string? Category { get; set; }
    }

    private static ServiceProvider CreateServices()
    {
        return new ServiceCollection()
            .AddSingleton<ISqlConnections>(new MockSqlConnections
            {
                OnNewByKey = _ => new MockDbConnection()
                    .InterceptExecuteReader(_ => new MockDbDataReader())
            })
            .BuildServiceProvider();
    }

    [Fact]
    public void RegisterDistinctValueScripts_Throws_For_Null_Arguments()
    {
        var manager = new MockDynamicScriptManager();
        var typeSource = new MockTypeSource();
        var services = CreateServices();

        Assert.Throws<ArgumentNullException>(() =>
            DistinctValuesRegistration.RegisterDistinctValueScripts(null!, typeSource, services));
        Assert.Throws<ArgumentNullException>(() =>
            DistinctValuesRegistration.RegisterDistinctValueScripts(manager, null!, services));
        Assert.Throws<ArgumentNullException>(() =>
            DistinctValuesRegistration.RegisterDistinctValueScripts(manager, typeSource, null!));
    }

    [Fact]
    public void RegisterDistinctValueScripts_Registers_For_Row()
    {
        var manager = new MockDynamicScriptManager();

        DistinctValuesRegistration.RegisterDistinctValueScripts(manager,
            new MockTypeSource(typeof(DistinctRow)), CreateServices());

        Assert.Contains(manager.GetRegisteredScriptNames(), x =>
            x.StartsWith("Lookup.Distinct.", StringComparison.Ordinal));
    }

    [Fact]
    public void RegisterDistinctValueScripts_Registers_For_Form_With_BasedOnRow()
    {
        var manager = new MockDynamicScriptManager();

        DistinctValuesRegistration.RegisterDistinctValueScripts(manager,
            new MockTypeSource(typeof(BasedForm)), CreateServices());

        Assert.Contains(manager.GetRegisteredScriptNames(), x =>
            x.StartsWith("Lookup.Distinct.", StringComparison.Ordinal));
    }

    [Fact]
    public void RegisterDistinctValueScripts_Registers_For_Form_With_Explicit_RowType()
    {
        var manager = new MockDynamicScriptManager();

        DistinctValuesRegistration.RegisterDistinctValueScripts(manager,
            new MockTypeSource(typeof(ExplicitForm)), CreateServices());

        Assert.Contains(manager.GetRegisteredScriptNames(), x =>
            x.StartsWith("Lookup.Distinct.", StringComparison.Ordinal));
    }

    [Fact]
    public void RegisterDistinctValueScripts_Throws_For_Non_Row_RowType()
    {
        var manager = new MockDynamicScriptManager();

        Assert.Throws<Exception>(() =>
            DistinctValuesRegistration.RegisterDistinctValueScripts(manager,
                new MockTypeSource(typeof(BadRowForm)), CreateServices()));
    }

    [Fact]
    public void RegisterDistinctValueScripts_Throws_When_RowType_Not_Specified()
    {
        var manager = new MockDynamicScriptManager();

        Assert.Throws<Exception>(() =>
            DistinctValuesRegistration.RegisterDistinctValueScripts(manager,
                new MockTypeSource(typeof(NoRowForm)), CreateServices()));
    }
}
