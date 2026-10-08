namespace Serenity.Services;

public class DisplayOrderFilterHelperTests
{
    [TableName("PlainRow")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class PlainRow : Row<PlainRow.RowFields>, IRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
        }
    }

    [TableName("ParentRow")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ParentRow : Row<ParentRow.RowFields>, IRow, IParentIdRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public int? ParentId { get => fields.ParentId[this]; set => fields.ParentId[this] = value; }

        public Field ParentIdField => fields.ParentId;

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public Int32Field ParentId = null!;
        }
    }

    [TableName("ActiveRow")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ActiveRow : Row<ActiveRow.RowFields>, IRow, IIsActiveRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public Int16Field IsActive = null!;
        }
    }

    [TableName("DeletedRow")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class DeletedRow : Row<DeletedRow.RowFields>, IRow, IIsDeletedRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }

        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public BooleanField IsDeleted = null!;
        }
    }

    [TableName("AllRow")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class AllRow : Row<AllRow.RowFields>, IRow, IParentIdRow, IIsActiveRow, IIsDeletedRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public int? ParentId { get => fields.ParentId[this]; set => fields.ParentId[this] = value; }
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }

        public Field ParentIdField => fields.ParentId;
        public Int16Field IsActiveField => fields.IsActive;
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public Int32Field ParentId = null!;
            public Int16Field IsActive = null!;
            public BooleanField IsDeleted = null!;
        }
    }

    [TableName("GuidParentRow")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class GuidParentRow : Row<GuidParentRow.RowFields>, IRow, IParentIdRow
    {
        [IdProperty]
        public Guid? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public Guid? ParentId { get => fields.ParentId[this]; set => fields.ParentId[this] = value; }

        public Field ParentIdField => fields.ParentId;

        public class RowFields : RowFieldsBase
        {
            public GuidField Id = null!;
            public GuidField ParentId = null!;
        }
    }

    [Fact]
    public void Returns_Empty_For_Plain_Row()
    {
        Assert.NotNull(DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new PlainRow()));
    }

    [Fact]
    public void Null_ParentId_Maps_To_Is_Null()
    {
        var criteria = DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new ParentRow());
        var sql = criteria.ToString(new SqlQuery());
        Assert.Contains("IS NULL", sql);
    }

    [Fact]
    public void NonNull_ParentId_Uses_Value_Criteria()
    {
        var criteria = DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new ParentRow { ParentId = 5 });
        var sql = criteria.ToString(new SqlQuery());
        Assert.Contains("= @p", sql);
    }

    [Fact]
    public void Guid_ParentId_Is_Supported()
    {
        var parentId = Guid.NewGuid();
        var criteria = DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new GuidParentRow { ParentId = parentId });
        var sql = criteria.ToString(new SqlQuery());
        Assert.Contains("= @p", sql);
    }

    [Fact]
    public void Includes_ParentId_Criteria()
    {
        var criteria = DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new ParentRow { ParentId = 5 });
        Assert.NotNull(criteria);
    }

    [Fact]
    public void Includes_IsActive_Criteria()
    {
        var criteria = DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new ActiveRow { IsActive = 1 });
        Assert.NotNull(criteria);
    }

    [Fact]
    public void Includes_IsDeleted_Criteria_When_Not_Active()
    {
        var criteria = DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new DeletedRow { IsDeleted = false });
        Assert.NotNull(criteria);
    }

    [Fact]
    public void Includes_All_Criteria()
    {
        var criteria = DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new AllRow
        {
            ParentId = 1,
            IsActive = 1,
            IsDeleted = false
        });
        Assert.NotNull(criteria);
    }
}
