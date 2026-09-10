namespace Serenity.Services;

public class ServiceQueryHelperTests
{
    [TableName("SortRow")]
    private class SortRow : Row<SortRow.RowFields>, IRow
    {
        [Size(50)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public int? Age { get => fields.Age[this]; set => fields.Age[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public StringField Name;
            public Int32Field Age;
#pragma warning restore CS0649
        }
    }

    [TableName("ActiveDeleted")]
    private class ActiveDeletedRow : Row<ActiveDeletedRow.RowFields>, IIsActiveDeletedRow
    {
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }
        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int16Field IsActive;
#pragma warning restore CS0649
        }
    }

    [TableName("ActiveDeletedNotNull")]
    private class ActiveDeletedNotNullRow : Row<ActiveDeletedNotNullRow.RowFields>, IIsActiveDeletedRow
    {
        [NotNull]
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }
        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int16Field IsActive;
#pragma warning restore CS0649
        }
    }

    [TableName("Deleted")]
    private class DeletedRow : Row<DeletedRow.RowFields>, IIsDeletedRow
    {
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public BooleanField IsDeleted;
#pragma warning restore CS0649
        }
    }

    [TableName("DeleteLog")]
    private class DeleteLogRow : Row<DeleteLogRow.RowFields>, IDeleteLogRow
    {
        public int? DeleteUserId { get => fields.DeleteUserId[this]; set => fields.DeleteUserId[this] = value; }
        public DateTime? DeleteDate { get => fields.DeleteDate[this]; set => fields.DeleteDate[this] = value; }

        public Field DeleteUserIdField => fields.DeleteUserId;
        public DateTimeField DeleteDateField => fields.DeleteDate;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field DeleteUserId;
            public DateTimeField DeleteDate;
#pragma warning restore CS0649
        }
    }

    [Fact]
    public void ApplySort_Throws_For_Null_Query()
    {
        SqlQuery? query = null;
        Assert.Throws<ArgumentNullException>(() => query.ApplySort("Name", false));
    }

    [Fact]
    public void ApplySort_Ignores_Null_Or_Empty_Sort()
    {
        var query = new SqlQuery().From(new SortRow()).Select("Name");
        Assert.Same(query, query.ApplySort(null, false));
    }

    [Fact]
    public void ApplySort_Uses_Query_Expression()
    {
        var query = new SqlQuery().Select("Name", "Name");
        query.ApplySort("Name", true);

        Assert.Contains("ORDER BY", query.ToString());
    }

    [Fact]
    public void ApplySort_Uses_Row_Field_Expression()
    {
        var query = new SqlQuery().From(new SortRow());
        query.ApplySort("Name", false);

        Assert.Contains("ORDER BY", query.ToString());
    }

    [Fact]
    public void ApplySort_SortBy_Ignores_Null_Or_Missing_Field()
    {
        var query = new SqlQuery().Select("Name", "Name");

        Assert.Same(query, query.ApplySort((SortBy)null));
        Assert.Same(query, query.ApplySort(new SortBy()));
    }

    [Fact]
    public void ApplySort_SortBy_Applies_Field()
    {
        var query = new SqlQuery().Select("Name", "Name");
        query.ApplySort(new SortBy("Name") { Descending = true });

        Assert.Contains("ORDER BY", query.ToString());
    }

    [Fact]
    public void ApplySort_SortByList_Uses_Defaults_When_Empty()
    {
        var query = new SqlQuery().Select("Name", "Name");
        query.ApplySort(null, new SortBy("Name"));

        Assert.Contains("ORDER BY", query.ToString());
    }

    [Fact]
    public void ApplySort_SortByList_Applies_List()
    {
        var query = new SqlQuery().Select("Name", "Name").Select("Age", "Age");
        query.ApplySort([new SortBy("Name")]);

        Assert.Contains("ORDER BY", query.ToString());
    }

    [Fact]
    public void ApplySkipTakeAndCount_Applies_Values()
    {
        var query = new SqlQuery().Select("Name", "Name");
        query.ApplySkipTakeAndCount(5, 10, false);

        Assert.Equal(5, query.Skip());
        Assert.Equal(10, query.Take());
        Assert.True(query.CountRecords);
    }

    [Fact]
    public void ApplySkipTakeAndCount_Does_Not_Count_When_Take_Zero()
    {
        var query = new SqlQuery().Select("Name", "Name");
        query.ApplySkipTakeAndCount(0, 0, false);

        Assert.False(query.CountRecords);
    }

    [Fact]
    public void ApplySkipTakeAndCount_Does_Not_Count_When_Excluded()
    {
        var query = new SqlQuery().Select("Name", "Name");
        query.ApplySkipTakeAndCount(0, 10, true);

        Assert.False(query.CountRecords);
    }

    [Fact]
    public void ApplyContainsText_Ignores_Null_Text()
    {
        var query = new SqlQuery().Select("Name", "Name");
        var called = false;
        Assert.Same(query, query.ApplyContainsText("  ", (_, _) => called = true));
        Assert.False(called);
    }

    [Fact]
    public void ApplyContainsText_Parses_Numeric_Id()
    {
        var query = new SqlQuery().Select("Name", "Name");
        string? text = null;
        long? id = null;
        query.ApplyContainsText("123", (t, i) => { text = t; id = i; });

        Assert.Equal("123", text);
        Assert.Equal(123L, id);
    }

    [Fact]
    public void ApplyContainsText_NonNumeric_Has_Null_Id()
    {
        var query = new SqlQuery().Select("Name", "Name");
        long? id = 5;
        query.ApplyContainsText("abc", (_, i) => id = i);

        Assert.Null(id);
    }

    [Fact]
    public void GetContainsTextFilter_Returns_Null_When_No_Text()
    {
        Assert.Null(ServiceQueryHelper.GetContainsTextFilter(null, [new Criteria("Name")]));
        Assert.Null(ServiceQueryHelper.GetContainsTextFilter("  ", [new Criteria("Name")]));
    }

    [Fact]
    public void GetContainsTextFilter_Returns_Null_When_No_Fields()
    {
        Assert.Null(ServiceQueryHelper.GetContainsTextFilter("abc", []));
    }

    [Fact]
    public void GetContainsTextFilter_Returns_Criteria_With_Fields()
    {
        Assert.NotNull(ServiceQueryHelper.GetContainsTextFilter("abc", [new Criteria("Name")]));
    }

    [Fact]
    public void GetNotDeletedCriteria_Returns_Null_For_Plain_Row()
    {
        Assert.Null(ServiceQueryHelper.GetNotDeletedCriteria(new SortRow()));
    }

    [Fact]
    public void GetNotDeletedCriteria_For_ActiveDeleted_Row()
    {
        Assert.NotNull(ServiceQueryHelper.GetNotDeletedCriteria(new ActiveDeletedRow()));
        Assert.NotNull(ServiceQueryHelper.GetNotDeletedCriteria(new ActiveDeletedNotNullRow()));
    }

    [Fact]
    public void GetNotDeletedCriteria_For_Deleted_Row()
    {
        Assert.NotNull(ServiceQueryHelper.GetNotDeletedCriteria(new DeletedRow()));
    }

    [Fact]
    public void GetNotDeletedCriteria_For_DeleteLog_Row()
    {
        Assert.NotNull(ServiceQueryHelper.GetNotDeletedCriteria(new DeleteLogRow()));
    }

    [Fact]
    public void UseSoftDelete_Detects_Interfaces()
    {
        Assert.False(ServiceQueryHelper.UseSoftDelete(new SortRow()));
        Assert.True(ServiceQueryHelper.UseSoftDelete(new ActiveDeletedRow()));
        Assert.True(ServiceQueryHelper.UseSoftDelete(new DeletedRow()));
        Assert.True(ServiceQueryHelper.UseSoftDelete(new DeleteLogRow()));
    }
}
