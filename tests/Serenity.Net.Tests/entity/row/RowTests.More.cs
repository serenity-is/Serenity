#pragma warning disable CS0649

namespace Serenity.Data;

public class RowTestsMore
{
    private class LocalRow : Row<LocalRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public LocalRow() : base()
        {
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
            }
        }
    }

    private class BigRow : Row<BigRow.RowFields>
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public BigRow() : base()
        {
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField[] Others;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
                Others = new StringField[65];
                for (var i = 0; i < Others.Length; i++)
                    Others[i] = new StringField(this, "Other" + i);
            }
        }
    }

    private class NoFactoryRow : Row<NoFactoryRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public NoFactoryRow() : base()
        {
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
            }
        }
    }

    [Fact]
    public void Typed_Interface_Members_Work()
    {
        IRow<LocalRow.RowFields> row = new LocalRow();

        Assert.NotNull(row.Fields);
        Assert.IsType<LocalRow>(row.CreateNew());
        Assert.IsType<LocalRow>(row.CloneRow());
    }

    [Fact]
    public void CreateNew_Without_RowFactory_Throws()
    {
        var row = new NoFactoryRow();
        row.GetFields().rowFactory = null;

        Assert.Throws<NotImplementedException>(() => ((IRow)row).CreateNew());
    }

    [Fact]
    public void Clone_Without_Tracking_Does_Not_Copy_Assignments()
    {
        var source = new LocalRow { Name = "A" };
        ((IRow)source).TrackAssignments = false;

        var clone = ((IRow)source).CloneRow();

        Assert.False(clone.TrackAssignments);
    }

    [Fact]
    public void Clone_Copies_Original_Dictionary_Previous_And_Validation_Data()
    {
        var row = new LocalRow { Name = "A" };
        ((IEditableObject)row).BeginEdit();
        ((IRow)row).SetDictionaryData("k", 5);
        ((INotifyPropertyChanged)row).PropertyChanged += (s, e) => { };
        ((IEditableRow)row).AddValidationError("Name", "error");

        var clone = ((IRow)row).CloneRow();

        Assert.True(((IEditableRow)clone).IsEditing);
        Assert.NotNull(((IEditableRow)clone).PreviousValues);
        Assert.True(((IEditableRow)clone).HasErrors);
        Assert.Equal(5, clone.GetDictionaryData("k"));
    }

    [Fact]
    public void OnFieldGet_Returns_When_Field_Is_Not_Null_But_Unassigned()
    {
        var row = new LocalRow { Name = "X" };
        row.ClearAssignment(row.GetFields().Name);
        ((IRow)row).TrackWithChecks = true;

        Assert.Equal("X", row.Name);
    }

    [Fact]
    public void OnFieldSet_Removes_Validation_Error()
    {
        var row = new LocalRow();
        ((IEditableRow)row).AddValidationError("Name", "error");
        Assert.True(((IEditableRow)row).HasErrors);

        row.Name = "Y";

        Assert.False(((IEditableRow)row).HasErrors);
    }

    [Fact]
    public void TrackAssignments_Toggling_With_PropertyChanged_Clones_Previous_Values()
    {
        var row = new LocalRow { Name = "A" };
        ((INotifyPropertyChanged)row).PropertyChanged += (s, e) => { };

        ((IRow)row).TrackAssignments = false;
        ((IRow)row).TrackAssignments = true;

        Assert.True(((IRow)row).TrackAssignments);
        Assert.NotNull(((IEditableRow)row).PreviousValues);
    }

    [Fact]
    public void TrackWithChecks_Enables_TrackAssignments()
    {
        var row = new LocalRow();
        ((IRow)row).TrackAssignments = false;

        ((IRow)row).TrackWithChecks = true;

        Assert.True(((IRow)row).TrackAssignments);
    }

    [Fact]
    public void Row_Indexer_Gets_And_Sets_Fields_And_Dictionary_Data()
    {
        IRow row = new LocalRow();

        Assert.Null(row["Name"]);
        Assert.Null(row["Unknown"]);

        row["Name"] = "A";
        Assert.Equal("A", row["Name"]);

        row.SetDictionaryData("extra", 5);
        Assert.Equal(5, row["extra"]);
        Assert.Null(row["missing"]);

        Assert.Throws<ArgumentOutOfRangeException>(() => row["NotAField"] = 1);
    }

    [Fact]
    public void SetDictionaryData_Handles_Null_And_Creates_Dictionary()
    {
        IRow row = new LocalRow();

        row.SetDictionaryData("k", null);
        Assert.Empty(row.GetDictionaryDataKeys());

        row.SetDictionaryData("k", 1);
        Assert.Equal(1, row.GetDictionaryData("k"));

        row.SetDictionaryData("k", null);
        Assert.Null(row.GetDictionaryData("k"));

        Assert.Null(((IRow)new LocalRow()).GetDictionaryData("k"));
    }

    [Fact]
    public void ClearAssignment_Early_Returns_And_Nulls_Empty_Array()
    {
        var row = new LocalRow();
        row.ClearAssignment(row.GetFields().Id);

        var big = new BigRow();
        var fields = big.GetFields();
        fields.Others[63][big] = "x";
        Assert.True(big.IsAssigned(fields.Others[63]));

        big.ClearAssignment(fields.Others[63]);
        Assert.False(big.IsAssigned(fields.Others[63]));
    }
}
