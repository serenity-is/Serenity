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
            public Int32Field ID = null!;
            public StringField Name = null!;
            public StringField NotMappedF = null!;
            public StringField TrimF = null!;
            public StringField TrimEmptyF = null!;
            public Int32Field Defaulted = null!;
        }
    }

    [TableName("NoIdRows")]
    private class NoIdRow : Row<NoIdRow.RowFields>
    {
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField Name = null!;
        }
    }

    [LocalTextPrefix("TextKeyLocal")]
    [DisplayText("Plural text", TextKey = "Site.TextKeyPlural")]
    [InstanceName("Singular text", TextKey = "Site.TextKeySingular")]
    private class TextKeyRow : Row<TextKeyRow.RowFields>
    {
        [CategoryName("Category text", TextKey = "Site.TextKeyCategory"),
            Tab("Tab text", TextKey = "Site.TextKeyTab"),
            Hint("Hint text", TextKey = "Site.TextKeyHint"),
            Placeholder("Placeholder text", TextKey = "Site.TextKeyPlaceholder")]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [DisplayText("Site.LiteralDisplay")]
        public string? LiteralDisplay { get => fields.LiteralDisplay[this]; set => fields.LiteralDisplay[this] = value; }

        [DisplayText("Invariant display", TextKey = "Site.InvariantDisplay")]
        public string? InvariantDisplay { get => fields.InvariantDisplay[this]; set => fields.InvariantDisplay[this] = value; }

        [DisplayName("Some Test")]
        public string? LegacyDisplay { get => fields.LegacyDisplay[this]; set => fields.LegacyDisplay[this] = value; }

        [DisplayName("Site.Key.Like")]
        public string? LegacyKeyLikeDisplay { get => fields.LegacyKeyLikeDisplay[this]; set => fields.LegacyKeyLikeDisplay[this] = value; }

        [DisplayText("Some Text")]
        public string? LiteralDisplayText { get => fields.LiteralDisplayText[this]; set => fields.LiteralDisplayText[this] = value; }

        [DisplayText(TextKey = "Site.KeyOnlyDisplay")]
        public string? KeyOnlyDisplay { get => fields.KeyOnlyDisplay[this]; set => fields.KeyOnlyDisplay[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField Name = null!;
            public StringField LiteralDisplay = null!;
            public StringField InvariantDisplay = null!;
            public StringField LegacyDisplay = null!;
            public StringField LegacyKeyLikeDisplay = null!;
            public StringField LiteralDisplayText = null!;
            public StringField KeyOnlyDisplay = null!;
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
        Assert.Throws<InvalidOperationException>(() => new NoIdRow().GetIdField());
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
    public void AddRowTexts_Registers_Explicit_Attribute_Text_Keys()
    {
        var registry = new LocalTextRegistry();
        var row = new TextKeyRow();

        Assert.Equal("Site.InvariantDisplay", TextKeyRow.Fields.InvariantDisplay.Caption.Key);
        Assert.Equal("Some Test", TextKeyRow.Fields.LegacyDisplay.Caption.Key);
        Assert.Equal("Site.Key.Like", TextKeyRow.Fields.LegacyKeyLikeDisplay.Caption.Key);
        Assert.Equal("Some Text", TextKeyRow.Fields.LiteralDisplayText.Caption.Key);

        registry.AddRowTexts([row], LocalText.InvariantLanguageID);

        Assert.Equal("Plural text", registry.TryGet(LocalText.InvariantLanguageID, "Site.TextKeyPlural", false));
        Assert.Equal("Singular text", registry.TryGet(LocalText.InvariantLanguageID, "Site.TextKeySingular", false));
        Assert.Equal("Category text", registry.TryGet(LocalText.InvariantLanguageID, "Site.TextKeyCategory", false));
        Assert.Equal("Tab text", registry.TryGet(LocalText.InvariantLanguageID, "Site.TextKeyTab", false));
        Assert.Equal("Hint text", registry.TryGet(LocalText.InvariantLanguageID, "Site.TextKeyHint", false));
        Assert.Equal("Placeholder text", registry.TryGet(LocalText.InvariantLanguageID, "Site.TextKeyPlaceholder", false));
        Assert.Equal("Site.LiteralDisplay", registry.TryGet(LocalText.InvariantLanguageID,
            "Db.TextKeyLocal.LiteralDisplay", false));
        Assert.Equal("Some Test", registry.TryGet(LocalText.InvariantLanguageID,
            "Db.TextKeyLocal.LegacyDisplay", false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID,
            "Site.Key.Like", false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID,
            "Db.TextKeyLocal.LegacyKeyLikeDisplay", false));
        Assert.Equal("Some Text", registry.TryGet(LocalText.InvariantLanguageID,
            "Db.TextKeyLocal.LiteralDisplayText", false));
        Assert.Equal("Invariant display", registry.TryGet(LocalText.InvariantLanguageID,
            "Site.InvariantDisplay", false));
        Assert.Equal("Db.TextKeyLocal.LegacyDisplay", TextKeyRow.Fields.LegacyDisplay.Caption.Key);
        Assert.Equal("Site.Key.Like", TextKeyRow.Fields.LegacyKeyLikeDisplay.Caption.Key);
        Assert.Equal("Db.TextKeyLocal.LiteralDisplayText", TextKeyRow.Fields.LiteralDisplayText.Caption.Key);
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, "Site.LiteralDisplay", false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, "Site.KeyOnlyDisplay", false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID,
            "Db.TextKeyLocal.KeyOnlyDisplay", false));
        Assert.Equal("Site.KeyOnlyDisplay", TextKeyRow.Fields.KeyOnlyDisplay.Caption.Key);
        Assert.Equal("Db.TextKeyLocal.LiteralDisplay", TextKeyRow.Fields.LiteralDisplay.Caption.Key);
        Assert.Equal("Site.InvariantDisplay", TextKeyRow.Fields.InvariantDisplay.Caption.Key);
    }

    [TableName("InstanceNameKeyLikeRows")]
    [LocalTextPrefix("InstanceNameKeyLike")]
    [InstanceName("Site.LegacyInstanceKey")]
    private class InstanceNameKeyLikeRow : Row<InstanceNameKeyLikeRow.RowFields>
    {
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField Name = null!;
        }
    }

    [Fact]
    public void AddRowTexts_Registers_KeyLike_InstanceName_As_Literal_Text()
    {
        var registry = new LocalTextRegistry();
        registry.AddRowTexts([new InstanceNameKeyLikeRow()], LocalText.InvariantLanguageID);

        Assert.Equal("Site.LegacyInstanceKey", registry.TryGet(LocalText.InvariantLanguageID,
            "Db.InstanceNameKeyLike.EntitySingular", false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, "Site.LegacyInstanceKey", false));
    }

    [Fact]
    public void OfJoin_Sets_Expression_And_Foreign_Flag()
    {
        var field = new StringField([], "F0");
        var join = new LeftJoin("Other", "O", new Criteria("1 = 1"));

        var result = field.OfJoin(join, "X");

        Assert.Same(field, result);
        Assert.Equal("O.X", field.Expression);
        Assert.Equal(FieldFlags.Foreign, (field.Flags & FieldFlags.Foreign));
    }

    [Fact]
    public void OfJoin_Throws_For_Null_Join()
    {
        var field = new StringField([], "F0");
        Assert.Throws<ArgumentNullException>(() => field.OfJoin(null!, "X"));
    }
}
