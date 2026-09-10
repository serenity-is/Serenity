namespace Serenity.Data;

public class ServiceRequestExtensionsTests
{
    private class UnboundRow : Row<UnboundRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField Hidden;

            public RowFields()
            {
                Hidden = new StringField(this, "Hidden");
            }
        }
    }

    private static StringField GetUnboundField()
    {
        return new UnboundRow.RowFields().Hidden;
    }

    [Fact]
    public void IncludeField_Adds_PropertyName_When_Available()
    {
        var field = new IdNameRow().GetFields().Name;
        Assert.Equal("Name", field.PropertyName);

        var request = new ListRequest();
        var result = request.IncludeField(field);

        Assert.Same(request, result);
        Assert.NotNull(request.IncludeColumns);
        Assert.Contains("Name", request.IncludeColumns);
    }

    [Fact]
    public void IncludeField_Falls_Back_To_Field_Name_When_PropertyName_IsNull()
    {
        var field = GetUnboundField();
        Assert.Null(field.PropertyName);

        var request = new ListRequest();
        request.IncludeField(field);

        Assert.Contains("Hidden", request.IncludeColumns);
    }

    [Fact]
    public void IncludeField_Creates_IncludeColumns_When_Null()
    {
        var request = new ListRequest { IncludeColumns = null };
        Assert.Null(request.IncludeColumns);

        request.IncludeField(new IdNameRow().GetFields().ID);

        Assert.NotNull(request.IncludeColumns);
        Assert.Single(request.IncludeColumns);
    }

    [Fact]
    public void IncludeField_Creates_CaseInsensitive_Set()
    {
        var field = new IdNameRow().GetFields().Name;

        var request = new ListRequest();
        request.IncludeField(field);

        Assert.Contains("name", request.IncludeColumns);
    }

    [Fact]
    public void IncludeField_Does_Not_Duplicate_Existing_Column()
    {
        var field = new IdNameRow().GetFields().Name;

        var request = new ListRequest { IncludeColumns = ["Name"] };
        request.IncludeField(field);
        request.IncludeField(field);

        Assert.Single(request.IncludeColumns);
    }
}
