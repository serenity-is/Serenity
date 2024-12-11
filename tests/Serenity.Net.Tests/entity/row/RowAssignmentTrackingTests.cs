namespace Serenity.Data;

public class RowAssignmentTrackingTests
{
    [Fact]
    public void HasAssignmentTracking_OnByDefault()
    {
        IRow row = new CityRow();
        Assert.True(row.TrackAssignments);
    }

    [Fact]
    public void NoneOfTheFields_AreAssigned_ByDefault()
    {
        var row = new CityRow();
        foreach (var field in row.GetFields())
            Assert.False(row.IsAssigned(field));
    }

    [Fact]
    public void Single_Field_CanBe_Assigned()
    {
        var row = new CityRow
        {
            CityName = "Test"
        };
        var fields = row.GetFields();
        Assert.True(row.IsAssigned(fields.CityName));
        foreach (var field in fields)
        {
            if (!ReferenceEquals(field, fields.CityName))
                Assert.False(row.IsAssigned(field));
            else
                Assert.True(row.IsAssigned(field));
        }
    }

    [Fact]
    public void Two_Fields_CanBe_Assigned()
    {
        var row = new CityRow
        {
            CityName = "Test",
            CityId = 5
        };
        Assert.True(row.IsAssigned(row.GetFields().CityName));
        Assert.True(row.IsAssigned(row.GetFields().CityId));
        var fields = row.GetFields();
        foreach (var field in fields)
        {
            if (!ReferenceEquals(field, fields.CityName) &&
                !ReferenceEquals(field, fields.CityId))
                Assert.False(row.IsAssigned(field));
            else
                Assert.True(row.IsAssigned(field));
        }
    }

    [Fact]
    public void ClearAssignment_Works()
    {
        var row = new CityRow
        {
            CityName = "Test",
            CityId = 5
        };
        var fields = row.GetFields();

        Assert.True(row.IsAssigned(fields.CityName));
        Assert.True(row.IsAssigned(fields.CityId));
        row.ClearAssignment(fields.CityName);
        Assert.False(row.IsAssigned(fields.CityName));
        Assert.True(row.IsAssigned(fields.CityId));

        foreach (var field in fields)
        {
            if (!ReferenceEquals(field, fields.CityId))
                Assert.False(row.IsAssigned(field));
            else
                Assert.True(row.IsAssigned(field));
        }
    }

    private class _199FieldsRow : Row<_199FieldsRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public StringField[] Others;

            public RowFields()
            {
                Id = new Int32Field(this, nameof(Id));
                Name = new StringField(this, nameof(Name));
                Others = new StringField[197];
                for (var i = 0; i < Others.Length; i++)
                    Others[i] = new StringField(this, "Other" + i);
            }
        }
    }


    [Fact]
    public void NoneOfTheFields_AreAssigned_ByDefault_199FieldsRow()
    {
        var row = new _199FieldsRow();
        foreach (var field in row.GetFields())
            Assert.False(row.IsAssigned(field));
    }

    [Fact]
    public void AssignFirstTwo_199FieldsRow()
    {
        var row = new _199FieldsRow()
        {
            Id = 1,
            Name = "Test"
        };
        var fields = row.GetFields();
        Assert.True(row.IsAssigned(fields.Id));
        Assert.True(row.IsAssigned(fields.Name));
        foreach (var field in fields)
        {
            if (!ReferenceEquals(field, fields.Id) &&
                !ReferenceEquals(field, fields.Name))
                Assert.False(row.IsAssigned(field));
            else
                Assert.True(row.IsAssigned(field));
        }
    }

    [Fact]
    public void Assign_IndexDivisibleBy5_199FieldsRow()
    {
        var row = new _199FieldsRow();
        var fields = row.GetFields();
        Assert.Equal(197, fields.Others.Length);
        for (var i = 0; i < fields.Others.Length; i++)
            if (i % 5 == 0)
                fields.Others[i][row] = "Test";

        for (var i = 0; i < fields.Others.Length; i++)
        {
            if (i % 5 == 0)
                Assert.True(row.IsAssigned(fields.Others[i]));
            else
                Assert.False(row.IsAssigned(fields.Others[i]));
        }
    }

    [Fact]
    public void Assign_IndexDivisibleBy5_ClearIndexDivisibleBy3_199FieldsRow()
    {
        var row = new _199FieldsRow();
        var fields = row.GetFields();
        Assert.Equal(197, fields.Others.Length);
        for (var i = 0; i < fields.Others.Length; i++)
            if (i % 5 == 0)
                fields.Others[i][row] = "Test";

        for (var i = 0; i < fields.Others.Length; i++)
            if (i % 3 == 0)
                row.ClearAssignment(fields.Others[i]);

        for (var i = 0; i < fields.Others.Length; i++)
        {
            if (i % 5 == 0 && i % 3 != 0)
                Assert.True(row.IsAssigned(fields.Others[i]));
            else
                Assert.False(row.IsAssigned(fields.Others[i]));
        }
    }

    [Fact]
    public void TrackAssignments_CanBeTurnedOff_199FieldsRow()
    {
        var row = new _199FieldsRow();
        var fields = row.GetFields();
        Assert.Equal(197, fields.Others.Length);
        for (var i = 0; i < fields.Others.Length; i++)
            if (i % 5 == 0)
                fields.Others[i][row] = "Test";

        for (var i = 0; i < fields.Others.Length; i++)
        {
            if (i % 5 == 0)
                Assert.True(row.IsAssigned(fields.Others[i]));
            else
                Assert.False(row.IsAssigned(fields.Others[i]));
        }

        ((IRow)row).TrackAssignments = false;
        foreach (var field in fields)
            Assert.False(row.IsAssigned(field));
    }

    [Fact]
    public void Clone_Preserves_Assignments_199FieldsRow_Case1()
    {
        var source = new _199FieldsRow();
        var fields = source.GetFields();
        source.Name = "Test";
        Assert.Equal(197, fields.Others.Length);
        for (var i = 0; i < fields.Others.Length; i++)
            if (i % 5 == 0)
                fields.Others[i][source] = "Test";

        var copy = source.Clone();
        Assert.False(copy.IsAssigned(fields.Id));
        Assert.True(copy.IsAssigned(fields.Name));

        for (var i = 0; i < fields.Others.Length; i++)
        {
            if (i % 5 == 0)
            {
                Assert.True(copy.IsAssigned(fields.Others[i]));
            }
            else
            {
                Assert.False(copy.IsAssigned(fields.Others[i]));
            }
        }
    }

    [Fact]
    public void Clone_Preserves_Assignments_199FieldsRow_Case2()
    {
        var source = new _199FieldsRow();
        var fields = source.GetFields();
        source.Name = "Test";

        Assert.False(source.IsAssigned(fields.Id));
        Assert.True(source.IsAssigned(fields.Name));

        for (var i = 0; i < fields.Others.Length; i++)
        {
            Assert.False(source.IsAssigned(fields.Others[i]));
        }

        var copy = source.Clone();
        Assert.False(copy.IsAssigned(fields.Id));
        Assert.True(copy.IsAssigned(fields.Name));

        for (var i = 0; i < fields.Others.Length; i++)
        {
            Assert.False(copy.IsAssigned(fields.Others[i]));
        }
    }
}
