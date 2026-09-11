using System.Threading;

namespace Serenity.PropertyGrid;

public class DefaultPropertyItemProviderMoreTests
{
    private static DefaultPropertyItemProvider CreateProvider(params Type[] types)
    {
        var services = new ServiceCollection();
        services.AddOptions();
        return new DefaultPropertyItemProvider(services.BuildServiceProvider(), new MockTypeSource(types));
    }

#pragma warning disable CS0649
    [TableName("PGRows2")]
    private class Row2 : Row<Row2.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public StringField Name;
        }
    }
#pragma warning restore CS0649

    [BasedOnRow(typeof(Row2), CheckNames = true)]
    private class CaseMismatchForm
    {
        public string NAME { get; set; }
    }

    private class PlainForm
    {
        public string Name { get; set; }
    }

    private class ChangeForm
    {
        [DisplayName("N")]
        public string Name { get; set; }
    }

    private class ChangeTrackingTypeSource : MockTypeSource, IChangeTokenProvider
    {
        private CancellationTokenSource cts = new();

        public ChangeTrackingTypeSource(params Type[] types) : base(types) { }

        public IChangeToken GetChangeToken() => new CancellationChangeToken(cts.Token);

        public void Fire()
        {
            var old = cts;
            cts = new CancellationTokenSource();
            old.Cancel();
        }
    }

    [Fact]
    public void Uses_Concrete_Row_As_BasedOnRow()
    {
        using var provider = CreateProvider();
        var items = provider.GetPropertyItemsFor(typeof(Row2), null).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void Returns_Null_BasedOnRow_For_Plain_Type()
    {
        using var provider = CreateProvider();
        var items = provider.GetPropertyItemsFor(typeof(PlainForm), null).ToList();
        Assert.Single(items);
    }

    [Fact]
    public void CheckNames_Throws_For_PropertyName_Mismatch()
    {
        using var provider = CreateProvider();
        Assert.Throws<InvalidProgramException>(() =>
            provider.GetPropertyItemsFor(typeof(CaseMismatchForm), null).ToList());
    }

    [Fact]
    public void Rebuilds_Processor_Factories_On_ChangeToken()
    {
        var typeSource = new ChangeTrackingTypeSource(typeof(BasicPropertyProcessor));
        var services = new ServiceCollection();
        services.AddOptions();
        using var provider = new DefaultPropertyItemProvider(services.BuildServiceProvider(), typeSource);

        var items = provider.GetPropertyItemsFor(typeof(ChangeForm), null).ToList();
        Assert.Equal("N", Assert.Single(items).Title);

        typeSource.Fire();

        items = provider.GetPropertyItemsFor(typeof(ChangeForm), null).ToList();
        Assert.Equal("N", Assert.Single(items).Title);
    }
}
