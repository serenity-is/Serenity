namespace Serenity.Data;

/// <summary>
/// The properties of a row get the attributes of the members of the interfaces they implement, including the mapping ones like
/// the joins, origins, columns and keys, so a group of properties can be shared between rows with their attributes.
/// </summary>
public class RowInterfaceAttributeTests
{
    public interface IRegionKey
    {
        [Identity]
        int? RegionKey { get; set; }

        [Column("RegionTitle"), Size(30)]
        string? Title { get; set; }
    }

    [TableName("Regions")]
    public class KeyedRegionRow : Row<KeyedRegionRow.RowFields>, IRegionKey
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field RegionKey = null!;
            public StringField Title = null!;
        }

        public int? RegionKey { get => fields.RegionKey[this]; set => fields.RegionKey[this] = value; }

        public string? Title { get => fields.Title[this]; set => fields.Title[this] = value; }
    }

    public interface IRegionReference
    {
        [DisplayName("Region"), ForeignKey(typeof(KeyedRegionRow)), LeftJoin("jRegion")]
        int? RegionId { get; set; }

        [Origin("jRegion", nameof(KeyedRegionRow.Title)), Size(50)]
        string? RegionTitle { get; set; }

        [Expression("T0.[A] + T0.[B]")]
        int? Total { get; set; }
    }

    public class ShopRow : Row<ShopRow.RowFields>, IRegionReference
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field RegionId = null!;
            public StringField RegionTitle = null!;
            public Int32Field Total = null!;
            public StringField Name = null!;
        }

        public int? RegionId { get => fields.RegionId[this]; set => fields.RegionId[this] = value; }

        // the own attribute wins over the one of the interface
        [Size(10)]
        public string? RegionTitle { get => fields.RegionTitle[this]; set => fields.RegionTitle[this] = value; }

        public int? Total { get => fields.Total[this]; set => fields.Total[this] = value; }

        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    }

    private static ShopRow.RowFields InitShop()
    {
        var fields = new ShopRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);
        return fields;
    }

    [Fact]
    public void Join_Of_The_Interface_Member_Is_Added_With_The_Key_Of_The_Joined_Row_From_Its_Interface()
    {
        var fields = InitShop();

        var join = Assert.Contains("jRegion", fields.Joins);
        Assert.Equal("Regions", join.Table);
        // the key of the joined row is its [Identity] property, which it has from its interface
        Assert.Contains("jRegion.[RegionKey]", join.OnCriteria!.ToString());
        Assert.Contains("T0.[RegionId]", join.OnCriteria!.ToString());
    }

    [Fact]
    public void Origin_Of_The_Interface_Member_Uses_The_Column_The_Joined_Row_Has_From_Its_Interface()
    {
        var fields = InitShop();

        Assert.Equal("jRegion.[RegionTitle]", fields.RegionTitle.Expression);
    }

    [Fact]
    public void Expression_And_Display_Name_Of_The_Interface_Member_Are_Used()
    {
        var fields = InitShop();

        Assert.Equal("T0.[A] + T0.[B]", fields.Total.Expression);
        Assert.Equal("Region", fields.RegionId.Caption?.Key);
    }

    [Fact]
    public void Own_Attribute_Of_The_Row_Property_Wins_Over_The_One_Of_The_Interface()
    {
        var fields = InitShop();

        Assert.Equal(10, fields.RegionTitle.Size);
    }

    [Fact]
    public void Column_And_Size_Of_The_Interface_Member_Are_Used_For_The_Field_Of_The_Joined_Row()
    {
        var fields = new KeyedRegionRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);

        Assert.Equal("RegionTitle", fields.Title.Name);
        Assert.Equal(30, fields.Title.Size);
        Assert.True(fields.RegionKey.Flags.HasFlag(FieldFlags.Identity));
    }

    [Fact]
    public void A_Property_Without_An_Interface_Is_Not_Affected()
    {
        var fields = InitShop();

        Assert.Equal("T0.[Name]", fields.Name.Expression);
        Assert.Equal(0, fields.Name.Size);
        Assert.Null(fields.Name.Join);
    }

    public interface ISelfReference
    {
        [ForeignKey(typeof(EmployeeRow)), LeftJoin("jManager")]
        int? ManagerId { get; set; }
    }

    [TableName("Employees")]
    public class EmployeeRow : Row<EmployeeRow.RowFields>, ISelfReference
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field EmployeeId = null!;
            public Int32Field ManagerId = null!;
        }

        [Identity]
        public int? EmployeeId { get => fields.EmployeeId[this]; set => fields.EmployeeId[this] = value; }

        public int? ManagerId { get => fields.ManagerId[this]; set => fields.ManagerId[this] = value; }
    }

    [Fact]
    public void Foreign_Key_To_The_Same_Row_Does_Not_Recurse_While_Finding_Its_Key()
    {
        // the constructor of the foreign key reads the attributes of the properties of the row it refers to, which has the
        // foreign key itself, so it only creates the attributes it looks for
        var fields = new EmployeeRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);

        var join = Assert.Contains("jManager", fields.Joins);
        Assert.Equal("Employees", join.Table);
        Assert.Contains("jManager.[EmployeeId]", join.OnCriteria!.ToString());
    }

    [Fact]
    public void GetPropertyAttribute_Resolves_As_WrappedProperty_Does()
    {
        var property = typeof(ShopRow).GetProperty(nameof(ShopRow.RegionTitle))!;

        Assert.Equal(10, property.GetPropertyAttribute<SizeAttribute>()?.Value);
        Assert.Equal(new Reflection.WrappedProperty(property).GetAttribute<SizeAttribute>()?.Value, property.GetPropertyAttribute<SizeAttribute>()?.Value);
        Assert.NotNull(property.GetPropertyAttribute<OriginAttribute>());
        Assert.Null(typeof(ShopRow).GetProperty(nameof(ShopRow.Name))!.GetPropertyAttribute<OriginAttribute>());
    }
}
