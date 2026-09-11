using System.Collections;

namespace Serenity.Web;

public class LookupScriptRegistrationTests
{
    [LookupScript("Registration.LookupRow")]
    private class RegLookupRow : Row<RegLookupRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    [LookupScript("Registration.Custom")]
    private class RegCustomLookup : LookupScript
    {
        protected override IEnumerable GetItems() => Array.Empty<object>();
    }

    [LookupScript("Registration.Bad")]
    private class RegBadLookup
    {
    }

    [LookupScript("Registration.Same")]
    private class DuplicateA : LookupScript
    {
        protected override IEnumerable GetItems() => Array.Empty<object>();
    }

    [LookupScript("Registration.Same")]
    private class DuplicateB : LookupScript
    {
        protected override IEnumerable GetItems() => Array.Empty<object>();
    }

    [LookupScript]
    private class RegAutoLookup : LookupScript
    {
        protected override IEnumerable GetItems() => Array.Empty<object>();
    }

    [LookupScript("Registration.External")]
    private class RegExternalLookup : LookupScript
    {
        protected override IEnumerable GetItems() => Array.Empty<object>();
    }

    [LookupScript(typeof(RegExternalLookup))]
    private class RegExternalRow : Row<RegExternalRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
#pragma warning restore CS0649
        }
    }

    [LookupScript("Registration.Generic")]
    private class RegGenericLookup<T> : LookupScript
        where T : class, IRow, new()
    {
        protected override IEnumerable GetItems() => Array.Empty<object>();
    }

    [LookupScript(typeof(RegGenericLookup<>))]
    private class RegGenericRow : Row<RegGenericRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
#pragma warning restore CS0649
        }
    }

    [LookupScript("Registration.Perm", Permission = "P", Expiration = 60)]
    private class RegPermLookup : LookupScript
    {
        protected override IEnumerable GetItems() => Array.Empty<object>();
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
    public void RegisterLookupScripts_Throws_For_Null_Arguments()
    {
        var manager = new MockDynamicScriptManager();
        var typeSource = new MockTypeSource();
        var services = CreateServices();

        Assert.Throws<ArgumentNullException>(() =>
            LookupScriptRegistration.RegisterLookupScripts(null!, typeSource, services));
        Assert.Throws<ArgumentNullException>(() =>
            LookupScriptRegistration.RegisterLookupScripts(manager, null!, services));
        Assert.Throws<ArgumentNullException>(() =>
            LookupScriptRegistration.RegisterLookupScripts(manager, typeSource, null!));
    }

    [Fact]
    public void RegisterLookupScripts_Registers_Row_And_Custom_Lookups()
    {
        var manager = new MockDynamicScriptManager();

        LookupScriptRegistration.RegisterLookupScripts(manager,
            new MockTypeSource(typeof(RegLookupRow), typeof(RegCustomLookup)), CreateServices());

        Assert.True(manager.IsRegistered("Lookup.Registration.LookupRow"));
        Assert.True(manager.IsRegistered("Lookup.Registration.Custom"));
    }

    [Fact]
    public void RegisterLookupScripts_Uses_Auto_Lookup_Key()
    {
        var manager = new MockDynamicScriptManager();

        LookupScriptRegistration.RegisterLookupScripts(manager,
            new MockTypeSource(typeof(RegAutoLookup)), CreateServices());

        var key = LookupScriptAttribute.AutoLookupKeyFor(typeof(RegAutoLookup));
        Assert.True(manager.IsRegistered("Lookup." + key));
    }

    [Fact]
    public void RegisterLookupScripts_Throws_For_Non_Lookup_Type()
    {
        var manager = new MockDynamicScriptManager();

        Assert.Throws<InvalidOperationException>(() =>
            LookupScriptRegistration.RegisterLookupScripts(manager,
                new MockTypeSource(typeof(RegBadLookup)), CreateServices()));
    }

    [Fact]
    public void RegisterLookupScripts_Throws_For_Duplicate_Keys()
    {
        var manager = new MockDynamicScriptManager();

        Assert.Throws<InvalidOperationException>(() =>
            LookupScriptRegistration.RegisterLookupScripts(manager,
                new MockTypeSource(typeof(DuplicateA), typeof(DuplicateB)), CreateServices()));
    }

    [Fact]
    public void RegisterLookupScripts_Skips_Row_With_External_Lookup_Type()
    {
        var manager = new MockDynamicScriptManager();

        LookupScriptRegistration.RegisterLookupScripts(manager,
            new MockTypeSource(typeof(RegExternalRow), typeof(RegExternalLookup)), CreateServices());

        Assert.True(manager.IsRegistered("Lookup.Registration.External"));
    }

    [Fact]
    public void RegisterLookupScripts_Creates_Generic_Lookup_Type()
    {
        var manager = new MockDynamicScriptManager();

        LookupScriptRegistration.RegisterLookupScripts(manager,
            new MockTypeSource(typeof(RegGenericRow)), CreateServices());

        Assert.True(manager.IsRegistered("Lookup.Registration.Generic"));
    }

    [Fact]
    public void RegisterLookupScripts_Applies_Permission_And_Expiration()
    {
        var manager = new MockDynamicScriptManager();

        LookupScriptRegistration.RegisterLookupScripts(manager,
            new MockTypeSource(typeof(RegPermLookup)), CreateServices());

        var script = (DynamicScript)manager.Registered["Lookup.Registration.Perm"];
        Assert.Equal("P", script.Permission);
        Assert.Equal(TimeSpan.FromSeconds(60), script.Expiration);
    }
}
