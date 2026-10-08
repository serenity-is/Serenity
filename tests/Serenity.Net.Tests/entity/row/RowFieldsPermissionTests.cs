namespace Serenity.Data;

public class RowFieldsPermissionTests
{
    private const string ModifyPerm = "Test:Modify";
    private const string UpdatePerm = "Test:Update";
    private const string InsertPerm = "Test:Insert";
    private const string ReadPerm = "Test:Read";

    [FieldModifyPermission(ModifyPerm)]
    public class ModifyOnlyRow : Row<ModifyOnlyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField PropField;
            public StringField QueryOnly;

            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                PropField = new StringField(this, "PropField");
                QueryOnly = new StringField(this, "QueryOnly");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string? PropField { get => fields.PropField[this]; set => fields.PropField[this] = value; }
        // QueryOnly deliberately has no matching row property.
    }

    [FieldModifyPermission(ModifyPerm)]
    [FieldReadPermission(ReadPerm)]
    public class ModifyAndReadRow : Row<ModifyAndReadRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField PropField;
            public StringField QueryOnly;

            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                PropField = new StringField(this, "PropField");
                QueryOnly = new StringField(this, "QueryOnly");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string? PropField { get => fields.PropField[this]; set => fields.PropField[this] = value; }
    }

    [FieldModifyPermission(ModifyPerm)]
    [FieldUpdatePermission(UpdatePerm)]
    [FieldInsertPermission(InsertPerm)]
    public class ExplicitRow : Row<ExplicitRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField QueryOnly;

            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                QueryOnly = new StringField(this, "QueryOnly");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    [FieldReadPermission(ReadPerm)]
    public class ReadOnlyRow : Row<ReadOnlyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField QueryOnly;

            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                QueryOnly = new StringField(this, "QueryOnly");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    private static TFields Fields<TFields>(TFields fields) where TFields : RowFieldsBase
    {
        fields.Initialize(annotations: null, dialect: SqlSettings.DefaultDialect, userEntityOptions: null);
        return fields;
    }

    [Fact]
    public void Update_Permission_Falls_Back_To_FieldModifyPermission_For_Property_Field()
    {
        var f = Fields(new ModifyOnlyRow.RowFields());

        Assert.Equal(ModifyPerm, f.PropField.UpdatePermission);
        Assert.Equal(ModifyPerm, f.PropField.InsertPermission);
        Assert.Null(f.PropField.ReadPermission);
    }

    [Fact]
    public void Update_Permission_Falls_Back_To_FieldModifyPermission_For_Non_Property_Field()
    {
        var f = Fields(new ModifyOnlyRow.RowFields());

        Assert.Equal(ModifyPerm, f.QueryOnly.UpdatePermission);
        Assert.Equal(ModifyPerm, f.QueryOnly.InsertPermission);
        Assert.Null(f.QueryOnly.ReadPermission);
    }

    [Fact]
    public void Update_Permission_Prefers_FieldModifyPermission_Over_FieldReadPermission()
    {
        var f = Fields(new ModifyAndReadRow.RowFields());

        Assert.Equal(ModifyPerm, f.QueryOnly.UpdatePermission);
        Assert.Equal(ModifyPerm, f.QueryOnly.InsertPermission);
        Assert.Equal(ReadPerm, f.QueryOnly.ReadPermission);
    }

    [Fact]
    public void Update_Permission_Prefers_FieldUpdatePermission_Over_FieldModifyPermission()
    {
        var f = Fields(new ExplicitRow.RowFields());

        Assert.Equal(UpdatePerm, f.QueryOnly.UpdatePermission);
        Assert.Equal(InsertPerm, f.QueryOnly.InsertPermission);
    }

    [Fact]
    public void Update_Permission_Falls_Back_To_FieldReadPermission_When_No_Modify()
    {
        var f = Fields(new ReadOnlyRow.RowFields());

        Assert.Equal(ReadPerm, f.QueryOnly.UpdatePermission);
        Assert.Equal(ReadPerm, f.QueryOnly.InsertPermission);
        Assert.Equal(ReadPerm, f.QueryOnly.ReadPermission);
    }

    [Fact]
    public async Task RowCreated_Applies_Permissions_Under_Concurrent_Row_Construction()
    {
        var fields = (RowFieldsBaseTestsMore.PermissionRow.RowFields)RowFieldsProvider.Current
            .Resolve(typeof(RowFieldsBaseTestsMore.PermissionRow.RowFields));

        await Task.WhenAll(Enumerable.Range(0, 64)
            .Select(_ => Task.Run(() => new RowFieldsBaseTestsMore.PermissionRow())));

        Assert.Equal("Perm", fields.Extra.ReadPermission);
    }
}
