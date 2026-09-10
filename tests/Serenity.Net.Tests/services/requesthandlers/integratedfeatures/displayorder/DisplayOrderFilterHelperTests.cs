namespace Serenity.Services;

public class DisplayOrderFilterHelperTests
{
    [TableName("PlainRow")]
    private class PlainRow : Row<PlainRow.RowFields>, IRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
#pragma warning restore CS0649
        }
    }

    [TableName("ParentRow")]
    private class ParentRow : Row<ParentRow.RowFields>, IRow, IParentIdRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public int? ParentId { get => fields.ParentId[this]; set => fields.ParentId[this] = value; }

        public Field ParentIdField => fields.ParentId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public Int32Field ParentId;
#pragma warning restore CS0649
        }
    }

    [TableName("ActiveRow")]
    private class ActiveRow : Row<ActiveRow.RowFields>, IRow, IIsActiveRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public Int16Field IsActive;
#pragma warning restore CS0649
        }
    }

    [TableName("DeletedRow")]
    private class DeletedRow : Row<DeletedRow.RowFields>, IRow, IIsDeletedRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }

        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public BooleanField IsDeleted;
#pragma warning restore CS0649
        }
    }

    [TableName("AllRow")]
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
#pragma warning disable CS0649
            public Int32Field ID;
            public Int32Field ParentId;
            public Int16Field IsActive;
            public BooleanField IsDeleted;
#pragma warning restore CS0649
        }
    }

    [Fact]
    public void Returns_Empty_For_Plain_Row()
    {
        Assert.NotNull(DisplayOrderFilterHelper.GetDisplayOrderFilterFor(new PlainRow()));
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
