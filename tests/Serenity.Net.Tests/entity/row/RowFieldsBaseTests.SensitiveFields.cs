namespace Serenity.Data;

public class RowFieldsBaseSensitiveFieldTests
{
    #region test rows

    public class SensitiveFieldsRow : Row<SensitiveFieldsRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string? PasswordHash { get => fields.PasswordHash[this]; set => fields.PasswordHash[this] = value; }
        public string? PasswordSalt { get => fields.PasswordSalt[this]; set => fields.PasswordSalt[this] = value; }
        public string? ApiKey { get => fields.ApiKey[this]; set => fields.ApiKey[this] = value; }
        public string? Normal { get => fields.Normal[this]; set => fields.Normal[this] = value; }

        [NotMapped]
        public string? Password { get => fields.Password[this]; set => fields.Password[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField PasswordHash = null!;
            public StringField PasswordSalt = null!;
            public StringField ApiKey = null!;
            public StringField Normal = null!;
            public StringField Password = null!;
        }
    }

    public class OverriddenSensitiveFieldsRow : Row<OverriddenSensitiveFieldsRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [MinSelectLevel(SelectLevel.List), Insertable(true), Updatable(true), Sortable(true), Filterable(true)]
        public string? PasswordHash { get => fields.PasswordHash[this]; set => fields.PasswordHash[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField PasswordHash = null!;
        }
    }

    public class ColumnNamedSensitiveFieldsRow : Row<ColumnNamedSensitiveFieldsRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [Column("password_hash")]
        public string? Hash { get => fields.Hash[this]; set => fields.Hash[this] = value; }

        [Column("[PasswordSalt]")]
        public string? Bracketed { get => fields.Bracketed[this]; set => fields.Bracketed[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Hash = null!;
            public StringField Bracketed = null!;
        }
    }

    public class FilterableFlagsRow : Row<FilterableFlagsRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [Filterable(false)]
        public string? NotFilterableByAttr { get => fields.NotFilterableByAttr[this]; set => fields.NotFilterableByAttr[this] = value; }

        [NotFilterable]
        public string? NotFilterableMarker { get => fields.NotFilterableMarker[this]; set => fields.NotFilterableMarker[this] = value; }

        [Filterable(true)]
        public string? FilterableByAttr { get => fields.FilterableByAttr[this]; set => fields.FilterableByAttr[this] = value; }

        public string? DefaultField { get => fields.DefaultField[this]; set => fields.DefaultField[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField NotFilterableByAttr = null!;
            public StringField NotFilterableMarker = null!;
            public StringField FilterableByAttr = null!;
            public StringField DefaultField = null!;
        }
    }

    #endregion

    private static TFields Init<TFields>() where TFields : RowFieldsBase, new()
    {
        var fields = new TFields();
        fields.Initialize(annotations: null, dialect: SqlSettings.DefaultDialect, userEntityOptions: null);
        return fields;
    }

    [Fact]
    public void SensitiveFieldNames_Infer_Never_SelectLevel()
    {
        var f = Init<SensitiveFieldsRow.RowFields>();

        Assert.Equal(SelectLevel.Never, f.PasswordHash.MinSelectLevel);
        Assert.Equal(SelectLevel.Never, f.PasswordSalt.MinSelectLevel);
        Assert.Equal(SelectLevel.Never, f.ApiKey.MinSelectLevel);
        Assert.Equal(SelectLevel.Auto, f.Normal.MinSelectLevel);
        Assert.Equal(SelectLevel.Auto, f.Id.MinSelectLevel);
    }

    [Fact]
    public void SensitiveFieldNames_Infer_NotInsertable_And_NotUpdatable()
    {
        var f = Init<SensitiveFieldsRow.RowFields>();

        Assert.False(f.PasswordHash.Flags.HasFlag(FieldFlags.Insertable));
        Assert.False(f.PasswordHash.Flags.HasFlag(FieldFlags.Updatable));
        Assert.False(f.PasswordSalt.Flags.HasFlag(FieldFlags.Insertable));
        Assert.False(f.ApiKey.Flags.HasFlag(FieldFlags.Updatable));

        Assert.True(f.Normal.Flags.HasFlag(FieldFlags.Insertable));
        Assert.True(f.Normal.Flags.HasFlag(FieldFlags.Updatable));
    }

    [Fact]
    public void SensitiveFieldNames_Infer_DenyFiltering()
    {
        var f = Init<SensitiveFieldsRow.RowFields>();

        Assert.True(f.PasswordHash.Flags.HasFlag(FieldFlags.DenyFiltering));
        Assert.True(f.PasswordSalt.Flags.HasFlag(FieldFlags.DenyFiltering));
        Assert.False(f.Normal.Flags.HasFlag(FieldFlags.DenyFiltering));
    }

    [Fact]
    public void SensitiveFieldNames_Add_Synthetic_Attributes()
    {
        var f = Init<SensitiveFieldsRow.RowFields>();

        Assert.Equal(SelectLevel.Never, f.PasswordHash.GetAttribute<MinSelectLevelAttribute>()!.Value);
        Assert.False(f.PasswordHash.GetAttribute<InsertableAttribute>()!.Value);
        Assert.False(f.PasswordHash.GetAttribute<UpdatableAttribute>()!.Value);
        Assert.False(f.PasswordHash.GetAttribute<SortableAttribute>()!.Value);
        Assert.False(f.PasswordHash.GetAttribute<FilterableAttribute>()!.Value);

        Assert.Null(f.Normal.GetAttribute<MinSelectLevelAttribute>());
        Assert.Null(f.Normal.GetAttribute<InsertableAttribute>());
        Assert.Null(f.Normal.GetAttribute<SortableAttribute>());
        Assert.Null(f.Normal.GetAttribute<FilterableAttribute>());
    }

    [Fact]
    public void NotMapped_SensitiveField_Keeps_Insertable_And_Updatable()
    {
        var f = Init<SensitiveFieldsRow.RowFields>();

        Assert.Equal(SelectLevel.Never, f.Password.MinSelectLevel);
        Assert.True(f.Password.Flags.HasFlag(FieldFlags.NotMapped));
        Assert.True(f.Password.Flags.HasFlag(FieldFlags.Insertable));
        Assert.True(f.Password.Flags.HasFlag(FieldFlags.Updatable));
    }

    [Fact]
    public void Explicit_Attributes_Override_Sensitive_Inference()
    {
        var f = Init<OverriddenSensitiveFieldsRow.RowFields>();

        Assert.Equal(SelectLevel.List, f.PasswordHash.MinSelectLevel);
        Assert.True(f.PasswordHash.Flags.HasFlag(FieldFlags.Insertable));
        Assert.True(f.PasswordHash.Flags.HasFlag(FieldFlags.Updatable));
        Assert.False(f.PasswordHash.Flags.HasFlag(FieldFlags.DenyFiltering));

        Assert.True(f.PasswordHash.GetAttribute<MinSelectLevelAttribute>()!.Value == SelectLevel.List);
        Assert.True(f.PasswordHash.GetAttribute<InsertableAttribute>()!.Value);
        Assert.True(f.PasswordHash.GetAttribute<UpdatableAttribute>()!.Value);
        Assert.True(f.PasswordHash.GetAttribute<SortableAttribute>()!.Value);
        Assert.True(f.PasswordHash.GetAttribute<FilterableAttribute>()!.Value);
    }

    [Fact]
    public void Sensitive_Column_Names_Are_Detected_Unbracketed()
    {
        var f = Init<ColumnNamedSensitiveFieldsRow.RowFields>();

        Assert.Equal(SelectLevel.Never, f.Hash.MinSelectLevel);
        Assert.Equal(SelectLevel.Never, f.Bracketed.MinSelectLevel);
    }

    [Fact]
    public void Filterable_Attribute_Adds_Or_Removes_DenyFiltering_Flag()
    {
        var f = Init<FilterableFlagsRow.RowFields>();

        Assert.True(f.NotFilterableByAttr.Flags.HasFlag(FieldFlags.DenyFiltering));
        Assert.True(f.NotFilterableMarker.Flags.HasFlag(FieldFlags.DenyFiltering));
        Assert.False(f.FilterableByAttr.Flags.HasFlag(FieldFlags.DenyFiltering));
        Assert.False(f.DefaultField.Flags.HasFlag(FieldFlags.DenyFiltering));
    }
}
