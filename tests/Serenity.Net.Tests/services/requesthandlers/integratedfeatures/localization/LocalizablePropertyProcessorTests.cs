namespace Serenity.PropertyGrid;

public class LocalizablePropertyProcessorTests
{
    [TableName("LocMain")]
    [LocalizationRow(typeof(LocMainLangRow), MappedIdField = "MasterId")]
    private class LocMainRow : Row<LocMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [Localizable(true)]
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public string Description { get => fields.Description[this]; set => fields.Description[this] = value; }

        [Localizable(false)]
        public string Secret { get => fields.Secret[this]; set => fields.Secret[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
            public StringField Description;
            public StringField Secret;
#pragma warning restore CS0649
        }
    }

    [TableName("LocMainLang")]
    private class LocMainLangRow : Row<LocMainLangRow.RowFields>, IIdRow, ILocalizationRow
    {
        [Identity]
        public long? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public int? MasterId { get => fields.MasterId[this]; set => fields.MasterId[this] = value; }
        public string LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public string Description { get => fields.Description[this]; set => fields.Description[this] = value; }

        public StringField CultureIdField => fields.LanguageId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int64Field Id;
            public Int32Field MasterId;
            public StringField LanguageId;
            public StringField Name;
            public StringField Description;
#pragma warning restore CS0649
        }
    }

    [TableName("Plain")]
    private class PlainRow : Row<PlainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    [TableName("BadLoc")]
    [LocalizationRow(typeof(PlainRow))]
    private class BadLocRow : Row<BadLocRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
#pragma warning restore CS0649
        }
    }

    private static PropertyInfoSource Source(string propertyName, IRow row)
    {
        return new PropertyInfoSource(typeof(LocMainRow).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public), row);
    }

    private static LocalizablePropertyProcessor Processor(IRow? row)
    {
        var processor = new LocalizablePropertyProcessor { BasedOnRow = row };
        processor.Initialize();
        return processor;
    }

    [Fact]
    public void Initialize_Does_Nothing_Without_BasedOnRow()
    {
        var processor = new LocalizablePropertyProcessor();
        processor.Initialize();
        Assert.False(processor.IsLocalized(new LocMainRow().GetFields().Name));
    }

    [Fact]
    public void Initialize_Does_Nothing_Without_Attribute()
    {
        var processor = Processor(new PlainRow());
        Assert.False(processor.IsLocalized(new PlainRow().GetFields().Name));
    }

    [Fact]
    public void Initialize_Throws_When_Local_Row_Not_LocalizationRow()
    {
        var processor = new LocalizablePropertyProcessor { BasedOnRow = new BadLocRow() };
        Assert.Throws<InvalidOperationException>(() => processor.Initialize());
    }

    [Fact]
    public void IsLocalized_Returns_True_For_Matching_Field()
    {
        var processor = Processor(new LocMainRow());
        Assert.True(processor.IsLocalized(new LocMainRow().GetFields().Description));
    }

    [Fact]
    public void IsLocalized_Returns_False_For_Id_Field()
    {
        var processor = Processor(new LocMainRow());
        Assert.False(processor.IsLocalized(new LocMainRow().GetFields().Id));
    }

    [Fact]
    public void IsLocalized_Returns_False_After_Second_Call_Uses_Cache()
    {
        var processor = Processor(new LocMainRow());
        Assert.True(processor.IsLocalized(new LocMainRow().GetFields().Description));
        Assert.True(processor.IsLocalized(new LocMainRow().GetFields().Description));
    }

    [Fact]
    public void Process_Sets_Localizable_From_Attribute()
    {
        var processor = Processor(new LocMainRow());
        var item = new PropertyItem();
        processor.Process(Source(nameof(LocMainRow.Name), new LocMainRow()), item);
        Assert.True(item.Localizable);
    }

    [Fact]
    public void Process_Does_Not_Set_Localizable_When_Attribute_False()
    {
        var processor = Processor(new LocMainRow());
        var item = new PropertyItem();
        processor.Process(Source(nameof(LocMainRow.Secret), new LocMainRow()), item);
        Assert.Null(item.Localizable);
    }

    [Fact]
    public void Process_Sets_Localizable_From_BasedOnField()
    {
        var processor = Processor(new LocMainRow());
        var item = new PropertyItem();
        processor.Process(Source(nameof(LocMainRow.Description), new LocMainRow()), item);
        Assert.True(item.Localizable);
    }

    [Fact]
    public void Process_Does_Nothing_Without_Attribute_Or_Field()
    {
        var processor = Processor(new PlainRow());
        var item = new PropertyItem();
        var source = new PropertyInfoSource(
            typeof(PlainRow).GetProperty(nameof(PlainRow.Name), BindingFlags.Instance | BindingFlags.Public),
            new PlainRow());
        processor.Process(source, item);
        Assert.Null(item.Localizable);
    }

    [Fact]
    public void Priority_Is_15()
    {
        Assert.Equal(15, new LocalizablePropertyProcessor().Priority);
    }
}
