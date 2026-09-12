namespace Serenity.Extensions.Entities;

public class UserPreferenceRowTests
{
    [Fact]
    public void Row_Properties_Roundtrip()
    {
        var row = new UserPreferenceRow
        {
            UserPreferenceId = 1,
            UserId = 2,
            PreferenceType = "type",
            Name = "name",
            Value = "value"
        };

        Assert.Equal(1, row.UserPreferenceId);
        Assert.Equal(2, row.UserId);
        Assert.Equal("type", row.PreferenceType);
        Assert.Equal("name", row.Name);
        Assert.Equal("value", row.Value);
        Assert.Same(row.GetFields().UserPreferenceId, ((IIdRow)row).IdField);
        Assert.IsAssignableFrom<INameRow>(row);
    }

    [Fact]
    public void Fields_Have_Expected_Names()
    {
        Assert.Equal("UserPreferenceId", UserPreferenceRow.Fields.UserPreferenceId.Name);
        Assert.Equal("UserId", UserPreferenceRow.Fields.UserId.Name);
        Assert.Equal("PreferenceType", UserPreferenceRow.Fields.PreferenceType.Name);
        Assert.Equal("Name", UserPreferenceRow.Fields.Name.Name);
        Assert.Equal("Value", UserPreferenceRow.Fields.Value.Name);
    }
}
