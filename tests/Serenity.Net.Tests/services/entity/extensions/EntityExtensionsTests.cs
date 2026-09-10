using Serenity.Localization;

namespace Serenity.Data;

public class EntityExtensionsTests
{
    [TableName("ExtRows")]
    [LocalTextPrefix("ExtLocal")]
    [DisplayName("ExtRowsPlural")]
    [InstanceName("ExtRowSingular")]
    private class ExtRow : Row<ExtRow.RowFields>, IIdRow, INameRow
    {
        [Identity, IdProperty, DisplayName("Id Caption")]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NameProperty, Category("Cat"), Tab("Tab1"), Hint("SomeHint"), Placeholder("SomePh")]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [NotMapped]
        public string? NotMappedF { get => fields.NotMappedF[this]; set => fields.NotMappedF[this] = value; }

        [SetFieldFlags(FieldFlags.Trim)]
        public string? TrimF { get => fields.TrimF[this]; set => fields.TrimF[this] = value; }

        [SetFieldFlags(FieldFlags.Trim | FieldFlags.TrimToEmpty)]
        public string? TrimEmptyF { get => fields.TrimEmptyF[this]; set => fields.TrimEmptyF[this] = value; }

        [DefaultValue(7)]
        public int? Defaulted { get => fields.Defaulted[this]; set => fields.Defaulted[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
            public StringField NotMappedF;
            public StringField TrimF;
            public StringField TrimEmptyF;
            public Int32Field Defaulted;
#pragma warning restore CS0649
        }
    }

    [TableName("NoIdRows")]
    private class NoIdRow : Row<NoIdRow.RowFields>
    {
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    [Fact]
    public void IsTableField_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => EntityFieldExtensions.IsTableField(null!));
    }

    [Fact]
    public void IsTableField_Returns_False_For_NotMapped()
    {
        Assert.True(ExtRow.Fields.Name.IsTableField());
        Assert.False(ExtRow.Fields.NotMappedF.IsTableField());
    }

    [Fact]
    public void EnumerateTableFields_Skips_NonTable_Fields()
    {
        var row = new ExtRow();
        var fields = row.EnumerateTableFields().ToList();

        Assert.Contains(ExtRow.Fields.Name, fields);
        Assert.DoesNotContain(ExtRow.Fields.NotMappedF, fields);
    }

    [Fact]
    public void GetTableFields_Skips_NonTable_Fields()
    {
        var tableFields = new ExtRow().GetTableFields();

        Assert.Contains(ExtRow.Fields.Name, tableFields);
        Assert.DoesNotContain(ExtRow.Fields.NotMappedF, tableFields);
    }

    [Fact]
    public void AutoTrim_Trims_To_Null()
    {
        var row = new ExtRow { TrimF = "  x  " };
        ExtRow.Fields.TrimF.AutoTrim(row);
        Assert.Equal("x", row.TrimF);
    }

    [Fact]
    public void AutoTrim_Trims_To_Empty()
    {
        var row = new ExtRow { TrimEmptyF = "   " };
        ExtRow.Fields.TrimEmptyF.AutoTrim(row);
        Assert.Equal("", row.TrimEmptyF);
    }

    [Fact]
    public void GetAttribute_And_GetAttributes_Handle_Null_And_Empty()
    {
        Assert.Null(EntityFieldExtensions.GetAttribute<CategoryAttribute>(null!));
        Assert.Null(ExtRow.Fields.NotMappedF.GetAttribute<CategoryAttribute>());
        Assert.NotNull(ExtRow.Fields.Name.GetAttribute<CategoryAttribute>());
        Assert.Empty(EntityFieldExtensions.GetAttributes<CategoryAttribute>(null!));
        Assert.Empty(ExtRow.Fields.NotMappedF.GetAttributes<CategoryAttribute>());
        Assert.Single(ExtRow.Fields.Name.GetAttributes<CategoryAttribute>());
    }

    [Fact]
    public void ApplyDefaultValues_Sets_Field_Defaults()
    {
        var row = new ExtRow();
        row.ApplyDefaultValues();
        Assert.Equal(7, row.Defaulted);
    }

    [Fact]
    public void ApplyDefaultValues_UnassignedOnly_Skips_Assigned()
    {
        var row = new ExtRow { Defaulted = 3 };
        row.ApplyDefaultValues(unassignedOnly: true);
        Assert.Equal(3, row.Defaulted);
    }

    [Fact]
    public void ApplyDefaultValues_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => RowExtensions.ApplyDefaultValues<ExtRow>(null!));
    }

    [Fact]
    public void GetIdField_Throws_When_No_Id_Field()
    {
        Assert.Throws<InvalidOperationException>(() => ((IRow)new NoIdRow()).GetIdField());
        Assert.Throws<ArgumentNullException>(() => RowExtensions.GetIdField(null!));
    }

    [Fact]
    public void FindField_And_FindFieldByPropertyName()
    {
        var row = new ExtRow();
        Assert.Null(row.FindField(null));
        Assert.Same(ExtRow.Fields.Name, row.FindField(ExtRow.Fields.Name.Name));
        Assert.Same(ExtRow.Fields.Name, row.FindFieldByPropertyName(nameof(ExtRow.Name)));
        Assert.NotNull(row.GetFields());
    }

    [Fact]
    public void As_Returns_Same_For_Same_Alias()
    {
        var fields = IdNameRow.Fields;
        Assert.Same(fields, fields.As(fields.AliasName));
    }

    [Fact]
    public void As_Throws_For_Whitespace_Alias()
    {
        Assert.Throws<ArgumentNullException>(() => IdNameRow.Fields.As(" "));
    }

    [Fact]
    public void As_Resolves_Aliased_Fields()
    {
        var aliased = IdNameRow.Fields.As("X1");
        Assert.Equal("X1", aliased.AliasName);
    }

    [Fact]
    public void AddRowTexts_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            EntityLocalTexts.AddRowTexts(null!, [new ExtRow()]));
        Assert.Throws<ArgumentNullException>(() =>
            new LocalTextRegistry().AddRowTexts(null!));
    }

    [Fact]
    public void AddRowTexts_Registers_Row_Texts()
    {
        var registry = new LocalTextRegistry();
        registry.AddRowTexts([new ExtRow()], LocalText.InvariantLanguageID);

        Assert.NotNull(registry.TryGet(LocalText.InvariantLanguageID,
            "Db.ExtLocal.EntityPlural", false));
        Assert.NotNull(registry.TryGet(LocalText.InvariantLanguageID,
            "Db.ExtLocal.EntitySingular", false));
        Assert.NotNull(registry.TryGet(LocalText.InvariantLanguageID,
            "Db.ExtLocal.Categories.Cat", false));
        Assert.NotNull(registry.TryGet(LocalText.InvariantLanguageID,
            "Db.ExtLocal.Tabs.Tab1", false));
    }

    [Fact]
    public void OfJoin_Sets_Expression_And_Foreign_Flag()
    {
        var field = new StringField([], "F0");
        var join = new LeftJoin("Other", "O", new Criteria("1 = 1"));

        var result = field.OfJoin(join, "X");

        Assert.Same(field, result);
        Assert.Equal("O.X", field.Expression);
        Assert.True((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign);
    }

    [Fact]
    public void OfJoin_Throws_For_Null_Join()
    {
        var field = new StringField([], "F0");
        Assert.Throws<ArgumentNullException>(() => field.OfJoin(null!, "X"));
    }
}
