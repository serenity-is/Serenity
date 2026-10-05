namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private class SpecialKeyRow : Row<SpecialKeyRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [UpdatePermission(SpecialPermissionKeys.LoggedIn)]
        public string? LoggedInUpdate { get => fields.LoggedInUpdate[this]; set => fields.LoggedInUpdate[this] = value; }

        [InsertPermission(SpecialPermissionKeys.LoggedIn)]
        public string? LoggedInInsert { get => fields.LoggedInInsert[this]; set => fields.LoggedInInsert[this] = value; }

        [ReadPermission(SpecialPermissionKeys.LoggedIn)]
        public string? LoggedInRead { get => fields.LoggedInRead[this]; set => fields.LoggedInRead[this] = value; }

        [UpdatePermission(SpecialPermissionKeys.Public)]
        public string? PublicUpdate { get => fields.PublicUpdate[this]; set => fields.PublicUpdate[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public StringField LoggedInUpdate = null!;
            public StringField LoggedInInsert = null!;
            public StringField LoggedInRead = null!;
            public StringField PublicUpdate = null!;
        }
    }

    private class SpecialKeyForm
    {
        public string? LoggedInUpdate { get; set; }
        public string? LoggedInInsert { get; set; }
        public string? LoggedInRead { get; set; }
        public string? PublicUpdate { get; set; }
    }

    private class AttributeOnlyForm
    {
        [UpdatePermission(SpecialPermissionKeys.LoggedIn)]
        public string? LoggedInUpdate { get; set; }

        [UpdatePermission(SpecialPermissionKeys.Public)]
        public string? PublicUpdate { get; set; }
    }

    [Fact]
    public void UpdatePermission_Field_Fallback_Keeps_LoggedIn()
    {
        var item = Process<SpecialKeyForm>(nameof(SpecialKeyForm.LoggedInUpdate), new SpecialKeyRow());

        Assert.Equal(SpecialPermissionKeys.LoggedIn, item.UpdatePermission);
    }

    [Fact]
    public void UpdatePermission_Field_Fallback_Drops_Public()
    {
        var item = Process<SpecialKeyForm>(nameof(SpecialKeyForm.PublicUpdate), new SpecialKeyRow());

        Assert.Null(item.UpdatePermission);
    }

    [Fact]
    public void InsertPermission_Field_Fallback_Keeps_LoggedIn()
    {
        var item = Process<SpecialKeyForm>(nameof(SpecialKeyForm.LoggedInInsert), new SpecialKeyRow());

        Assert.Equal(SpecialPermissionKeys.LoggedIn, item.InsertPermission);
    }

    [Fact]
    public void ReadPermission_Field_Fallback_Keeps_LoggedIn()
    {
        var item = Process<SpecialKeyForm>(nameof(SpecialKeyForm.LoggedInRead), new SpecialKeyRow());

        Assert.Equal(SpecialPermissionKeys.LoggedIn, item.ReadPermission);
    }

    [Fact]
    public void UpdatePermission_Attribute_Path_Keeps_LoggedIn()
    {
        var item = Process<AttributeOnlyForm>(nameof(AttributeOnlyForm.LoggedInUpdate));

        Assert.Equal(SpecialPermissionKeys.LoggedIn, item.UpdatePermission);
    }

    [Fact]
    public void UpdatePermission_Attribute_Path_Drops_Public()
    {
        var item = Process<AttributeOnlyForm>(nameof(AttributeOnlyForm.PublicUpdate));

        Assert.Null(item.UpdatePermission);
    }

    [Fact]
    public void UpdatePermission_Attribute_And_Field_Paths_Agree_On_LoggedIn()
    {
        var fromAttribute = Process<AttributeOnlyForm>(nameof(AttributeOnlyForm.LoggedInUpdate));
        var fromField = Process<SpecialKeyForm>(nameof(SpecialKeyForm.LoggedInUpdate), new SpecialKeyRow());

        Assert.Equal(fromAttribute.UpdatePermission, fromField.UpdatePermission);
    }
}
